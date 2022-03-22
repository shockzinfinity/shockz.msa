using shockz.msa.shopping.Aggregator.Extensions;
using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public class BasketService : IBasketService
  {
    private readonly HttpClient _httpClient;

    public BasketService(HttpClient httpClient)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<BasketModel> GetBasket(string userName)
    {
      var response = await _httpClient.GetAsync($"/api/v1/Basket/{userName}");

      return await response.ReadContentAs<BasketModel>();
    }
  }
}
