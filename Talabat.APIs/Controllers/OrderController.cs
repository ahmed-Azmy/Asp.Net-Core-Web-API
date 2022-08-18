using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrderController(IOrderService orderService , IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orderAddress = mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, orderAddress);
            if (order == null) return BadRequest(new ApiResponse(400, "An Error Occured During Creating"));

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await orderService.GetOrdersForUserAsync(buyerEmail);

            return Ok(mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var order = await orderService.GetOrderByIdForUser(id,buyerEmail);

            return Ok(mapper.Map<Order , OrderToReturnDto>(order));
        }
        [HttpGet("deliveryMethod")]
        public async Task<ActionResult<IReadOnlyList<Order>>> GetDeliveryMethods()
        {
            return Ok(await orderService.GetDeliveryMethodsAsync());
        }
    }
}
