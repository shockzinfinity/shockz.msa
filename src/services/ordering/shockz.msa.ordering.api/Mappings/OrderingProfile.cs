using AutoMapper;
using shockz.msa.eventBus.messages.Events;
using shockz.msa.ordering.application.Features.Orders.Commands.CheckoutOrder;

namespace shockz.msa.ordering.api.Mappings
{
  public class OrderingProfile : Profile
  {
    public OrderingProfile()
    {
      CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
    }
  }
}
