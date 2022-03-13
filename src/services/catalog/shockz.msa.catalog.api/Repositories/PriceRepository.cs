using MongoDB.Driver;
using shockz.msa.catalog.api.Data;
using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Repositories
{
  public class PriceRepository : IPriceRepository
  {
    private readonly ICatalogContext _context;

    public PriceRepository(ICatalogContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Price>> GetPricesByProductId(string productId)
    {
      FilterDefinition<Price> filterDefinition = Builders<Price>.Filter.Eq(p => p.ProductId, productId);

      return await _context.Prices.Find(filterDefinition).ToListAsync();
    }

    public async Task<Price> GetPrice(string id)
    {
      return await _context.Prices.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Price>> GetPricesBySupplier(string supplier)
    {
      FilterDefinition<Price> filterDefinition = Builders<Price>.Filter.Eq(p => p.Supplier, supplier);

      return await _context.Prices.Find(filterDefinition).ToListAsync();
    }

    public async Task CreatePrice(Price price)
    {
      await _context.Prices.InsertOneAsync(price);
    }

    public async Task<bool> UpdatePrice(Price price)
    {
      var updateResult = await _context.Prices.ReplaceOneAsync(filter: p => p.Id == price.Id, replacement: price);

      return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeletePrice(string id)
    {
      FilterDefinition<Price> filter = Builders<Price>.Filter.Eq(p => p.Id, id);
      DeleteResult deleteResult = await _context.Prices.DeleteOneAsync(filter);

      return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
  }
}
