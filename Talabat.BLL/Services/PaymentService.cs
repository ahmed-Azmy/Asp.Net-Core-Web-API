using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications.OrderSpecification;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order_Aggregate;
using Product = Talabat.DAL.Entities.Product;

namespace Talabat.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;

        public PaymentService(IConfiguration configuration,
                              IBasketRepository basketRepository,
                              IUnitOfWork unitOfWork)
        {
            this.configuration = configuration;
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:Secretkey"];

            var basket = await basketRepository.GetCustomerBasket(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
            }

            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.Repository<Product>().GetAsync(item.Id);
                if (item.Price != product.Price)
                    item.Price = product.Price;
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                intent = await service.CreateAsync(options);

                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            basket.ShippingPrice = shippingPrice;

            await basketRepository.UpdateCustomerBasket(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
            var spec = new OrderWithItemByPaymentIntentSpecifications(paymentIntentId);

            var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentReceived;

            unitOfWork.Repository<Order>().Update(order);

            await unitOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithItemByPaymentIntentSpecifications(paymentIntentId);

            var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;

            unitOfWork.Repository<Order>().Update(order);

            await unitOfWork.Complete();

            return order;
        }
    }
}
