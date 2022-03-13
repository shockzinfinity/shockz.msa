﻿using Microsoft.Extensions.Caching.Distributed;
using shockz.msa.basket.api.Entities;
using System.Text.Json;

namespace shockz.msa.basket.api.Repositories
{
  public class BasketRepository : IBasketRepository
  {
    private readonly IDistributedCache _redisCache;

    public BasketRepository(IDistributedCache redisCache)
    {
      _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
    }

    public async Task<ShoppingCart> GetBasket(string userName)
    {
      var basket = await _redisCache.GetStringAsync(userName); // json object
      if (string.IsNullOrEmpty(basket))
        return null;

      return JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
    {
      await _redisCache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));

      return await GetBasket(basket.UserName);
    }

    public async Task DeleteBasket(string userName)
    {
      await _redisCache.RemoveAsync(userName);
    }
  }
}
