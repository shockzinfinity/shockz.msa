using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Repositories
{
  public interface IPriceRepository
  {
    Task<IEnumerable<Price>> GetPricesByProductId(string productId);
    Task<Price> GetPrice(string id);
    Task<IEnumerable<Price>> GetPricesBySupplier(string supplier);
    Task CreatePrice(Price price);
    Task<bool> UpdatePrice(Price price);
    Task<bool> DeletePrice(string id);
  }
}
