using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public interface IBasketService
  {
    Task<BasketModel> GetBasket(string userName);
  }
}
