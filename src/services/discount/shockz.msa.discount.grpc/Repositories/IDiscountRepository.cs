using shockz.msa.discount.grpc.Entities;

namespace shockz.msa.discount.grpc.Repositories
{
  public interface IDiscountRepository
  {
    Task<Coupon> GetDiscount(string productName);
    Task<bool> CreateDiscount(Coupon coupon);
    Task<bool> UpdateDiscount(Coupon coupon);
    Task<bool> DeleteDiscount(string productName);
  }
}
