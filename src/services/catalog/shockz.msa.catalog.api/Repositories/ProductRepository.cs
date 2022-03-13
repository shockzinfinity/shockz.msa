using MongoDB.Driver;
using shockz.msa.catalog.api.Data;
using shockz.msa.catalog.api.Entities;

namespace shockz.msa.catalog.api.Repositories
{
  public class ProductRepository : IProductRepository
  {
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
      return await _context.Products.Find(p => true).ToListAsync();
    }

    public async Task<Product> GetProduct(string id)
    {
      return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByName(string name)
    {
      //FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.ElemMatch(p => p.Name, name);
      FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(p => p.Name, name);

      return await _context.Products.Find(filterDefinition).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
    {
      //FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.ElemMatch(p => p.Category, categoryName);
      FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

      return await _context.Products.Find(filterDefinition).ToListAsync();
    }

    public async Task CreateProduct(Product product)
    {
      await _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProduct(Product product)
    {
      var updateResult = await _context.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);

      return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProduct(string id)
    {
      FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
      DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter: filter);

      return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
  }
}
