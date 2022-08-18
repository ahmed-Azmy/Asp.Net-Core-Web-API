using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.APIs.DTOs;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.APIs.Helpers
{
    public class OrderItemUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration configuration;

        public OrderItemUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ItemOrdered.PictureUrl))
                return $"{configuration["ApiUrl"]}{source.ItemOrdered.PictureUrl}";
            return null;
        }
    }
}
