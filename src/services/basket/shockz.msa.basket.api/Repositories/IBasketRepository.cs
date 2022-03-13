using shockz.msa.basket.api.Entities;

namespace shockz.msa.basket.api.Repositories
{
  public interface IBasketRepository
  {
    Task<ShoppingCart> GetBasket(string userName);
    Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
    Task DeleteBasket(string userName);
  }
}
