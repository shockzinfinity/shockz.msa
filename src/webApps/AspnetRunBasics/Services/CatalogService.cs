using AspnetRunBasics.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services;

public class CatalogService : ICatalogService
{
  private readonly HttpClient _client;
  private readonly ILogger<CatalogService> _logger;

  public CatalogService(HttpClient client, ILogger<CatalogService> logger)
  {
    _client = client ?? throw new ArgumentNullException(nameof(client));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task<IEnumerable<CatalogModel>> GetCatalog()
  {
    _logger.LogDebug($"Getting catalog products from url: {_client.BaseAddress}");
    //_logger.LogInformation($"Getting catalog products from url: {_client.BaseAddress}");

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
