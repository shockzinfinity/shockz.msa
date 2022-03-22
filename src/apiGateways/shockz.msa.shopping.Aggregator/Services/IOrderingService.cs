using shockz.msa.shopping.Aggregator.Models;

namespace shockz.msa.shopping.Aggregator.Services
{
  public interface IOrderingService
  {
    Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
  }
}
