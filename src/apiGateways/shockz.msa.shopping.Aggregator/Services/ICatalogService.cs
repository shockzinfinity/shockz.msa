using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public interface ICatalogService
  {
    Task<IEnumerable<CatalogModel>> GetCatalog();
    Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);
    Task<CatalogModel> GetCatalogById(string id);
  }
}
