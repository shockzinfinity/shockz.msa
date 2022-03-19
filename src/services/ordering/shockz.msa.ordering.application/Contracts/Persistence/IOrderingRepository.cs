using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.application.Contracts.Persistence
{
  public interface IOrderingRepository : IAsyncRepository<Order>
  {
    Task<IEnumerable<Order>> GetOrderByUserName(string userName);
  }
}
