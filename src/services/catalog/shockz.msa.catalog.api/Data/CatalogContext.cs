using MongoDB.Driver;
using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Data
{
  public class CatalogContext : ICatalogContext
  {
    public CatalogContext(IConfiguration configuration)
    {
      var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
      var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

      Products = database.GetCollection<Product>("Products");
      Prices = database.GetCollection<Price>("Prices");

      CatalogContextSeed.SeedData(Products, Prices);
    }
    public IMongoCollection<Product> Products { get; }
    public IMongoCollection<Price> Prices { get; }
  }
}
