using AspnetRunBasics.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services;

public class OrderService : IOrderService
{
  private readonly HttpClient _client;

  public OrderService(HttpClient client)
  {
    _client = client ?? throw new ArgumentNullException(nameof(client));
  }

  public async Task<IEnumerable<OrderResponseModel>> GetOrderByUserName(string userName)
  {
    return await _client.GetFromJsonAsync<List<OrderResponseModel>>($"/Order/{userName}");
  }
}
