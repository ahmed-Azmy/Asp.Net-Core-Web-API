using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Talabat.APIs.DTOs;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities;

namespace Talabat.APIs.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string basketId)
        {
            var basket = await basketRepository.GetCustomerBasket(basketId);
            return Ok(basket ?? new CustomerBasket(basketId));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var customerBasket = await basketRepository.UpdateCustomerBasket(mappedBasket);
            return Ok(customerBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string basketId)
        {
            return await basketRepository.DeleteCustomerBasket(basketId);           
        }

    }
}
