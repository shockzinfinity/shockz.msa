using MongoDB.Driver;
using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Data
{
  public interface ICatalogContext
  {
    IMongoCollection<Product> Products { get; }
  }
}
