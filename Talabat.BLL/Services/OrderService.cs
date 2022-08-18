using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.BLL.Specifications.OrderSpecification;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;


        public OrderService(IBasketRepository basketRepository,
                            IUnitOfWork unitOfWork,
                            IPaymentService paymentService
                           )
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shipToAddress)
        {
            // 1. Get Basket From Basket Repository
            var basket =await basketRepository.GetCustomerBasket(basketId);
            // 2. Get Selected Item at Basket From Products Repo
            var orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var Product = await unitOfWork.Repository<Product>().GetAsync(item.Id);
                var ProductItemOrderd = new ProductItemOrdered(Product.Id, Product.Name, Product.PictureUrl);
                var OrderItem = new OrderItem(ProductItemOrderd, Product.Price, item.Quantity);
                orderItems.Add(OrderItem);
            }
            //3. Get delivery Method From DeliveryMethod Repository
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);
            //4. Calculate subtotal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);
            // Check If Order Exists
            var spec = new OrderWithItemByPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if(existingOrder != null)
            {
                unitOfWork.Repository<Order>().Delete(existingOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            }
            //5. Create Order
            var order = new Order(buyerEmail, shipToAddress, deliveryMethod, orderItems , subtotal , basket.PaymentIntentId);
            await unitOfWork.Repository<Order>().Add(order);
            //6. Save To Database
            int result = await unitOfWork.Complete();
            if (result <= 0) return null;

            return order;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

        public Task<Order> GetOrderByIdForUser(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(orderId, buyerEmail);

            var order = unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(buyerEmail);
            var orders = await unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }
    }
}
