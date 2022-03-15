using AutoMapper;
using shockz.msa.discount.grpc.Entities;
using shockz.msa.discount.grpc.Protos;

namespace shockz.msa.discount.grpc.MapperProfiles
{
  public class DiscountProfile : Profile
  {
    public DiscountProfile()
    {
      CreateMap<Coupon, CouponModel>().ReverseMap();
    }
  }
}
