using AspnetRunBasics.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services;

public class BasketService : IBasketService
{
  private readonly HttpClient _client;

  public BasketService(HttpClient client)
  {
    _client = client ?? throw new ArgumentNullException(nameof(client));
  }

  public async Task<BasketModel> GetBasket(string userName)
  {
    return await _client.GetFromJsonAsync<BasketModel>($"/Basket/{userName}");
  }

  public async Task<BasketModel> UpdateBasket(BasketModel basket)
  {
    var response = await _client.PostAsJsonAsync($"/Basket", basket);
    if (response.IsSuccessStatusCode)
      return await response.Content.ReadFromJsonAsync<BasketModel>();
    else
      throw new Exception("Something went wrong when calling api.");
  }

  public async Task CheckoutBasket(BasketCheckoutModel basketCheckout)
  {
    var response = await _client.PostAsJsonAsync($"/Basket/Checkout", basketCheckout);
    if (!response.IsSuccessStatusCode)
      throw new Exception("Something went wrong when calling api.");
  }
}
