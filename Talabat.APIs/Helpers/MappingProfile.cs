using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.APIs.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(D => D.ProductType, O => O.MapFrom(P => P.ProductType.Name))
                .ForMember(D => D.ProductBrand, O => O.MapFrom(P => P.ProductBrand.Name))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<PictureUrlResolver>());

            CreateMap<DAL.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<AddressDto, DAL.Entities.Order_Aggregate.Address>();

            CreateMap<Order, OrderToReturnDto>().
                ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName)).
                ForMember(d => d.DeliveryCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.ItemOrdered.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemUrlResolver>());
        }
    }
}
