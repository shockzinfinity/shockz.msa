using shockz.msa.shopping.Aggregator.Extensions;
using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public class OrderingService : IOrderingService
  {
    private readonly HttpClient _httpClient;

    public OrderingService(HttpClient httpClient)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
    {
      var response = await _httpClient.GetAsync($"/api/v1/Order/{userName}");

      return await response.ReadContentAs<List<OrderResponseModel>>();
    }
  }
}
