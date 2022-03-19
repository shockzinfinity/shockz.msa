using AutoMapper;
using shockz.msa.ordering.application.Features.Orders.Commands.CheckoutOrder;
using shockz.msa.ordering.application.Features.Orders.Commands.UpdateOrder;
using shockz.msa.ordering.application.Features.Orders.Queries.GetOrdersList;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.application.Mappings
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<Order, OrdersViewModel>().ReverseMap();
      CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
      CreateMap<Order, UpdateOrderCommand>().ReverseMap();
    }
  }
}
