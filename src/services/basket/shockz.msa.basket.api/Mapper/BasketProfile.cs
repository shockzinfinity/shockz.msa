using AutoMapper;
using shockz.msa.basket.api.Entities;
using shockz.msa.eventBus.messages.Events;

namespace shockz.msa.basket.api.Mapper
{
  public class BasketProfile : Profile
  {
    public BasketProfile()
    {
      CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
    }
  }
}
