using shockz.msa.shopping.Aggregator.Extensions;
using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public class CatalogService : ICatalogService
  {
    private readonly HttpClient _httpClient;

    public CatalogService(HttpClient httpClient)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalog()
    {
      var response = await _httpClient.GetAsync("/api/v1/Catalog");

      return await response.ReadContentAs<List<CatalogModel>>();
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
    {
      var response = await _httpClient.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}");

      return await response.ReadContentAs<List<CatalogModel>>();
    }

    public async Task<CatalogModel> GetCatalogById(string id)
    {
      var response = await _httpClient.GetAsync($"/api/v1/Product/{id}");

      return await response.ReadContentAs<CatalogModel>();
    }
  }
}
