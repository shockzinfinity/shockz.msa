using AspnetRunBasics.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services;

public class CatalogService : ICatalogService
{
  private readonly HttpClient _client;

  public CatalogService(HttpClient client)
  {
    _client = client ?? throw new ArgumentNullException(nameof(client));
  }

  public async Task<IEnumerable<CatalogModel>> GetCatalog()
  {
    return await _client.GetFromJsonAsync<List<CatalogModel>>("/Product");
  }

  public async Task<CatalogModel> GetCatalogById(string id)
  {
    return await _client.GetFromJsonAsync<CatalogModel>($"/Product/{id}");
  }

  public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
  {
    return await _client.GetFromJsonAsync<List<CatalogModel>>($"/Product/GetProductyByCategory/{category}");
  }

  public async Task<CatalogModel> CreateCatalog(CatalogModel catalog)
  {
    var response = await _client.PostAsJsonAsync($"/Product", catalog);
    if (response.IsSuccessStatusCode)
      return await response.Content.ReadFromJsonAsync<CatalogModel>();
    else
      throw new Exception("Something went wrong when calling api.");
  }
}
