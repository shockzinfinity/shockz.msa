using Microsoft.EntityFrameworkCore;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.domain.Entities;
using shockz.msa.ordering.infrastructure.Persistence;

namespace shockz.msa.ordering.infrastructure.Repositories
{
  public class OrderingRepository : RepositoryBase<Order>, IOrderingRepository
  {
    public OrderingRepository(OrderContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetOrderByUserName(string userName)
    {
      var orderList = await _context.Orders.Where(o => o.UserName == userName).ToListAsync();

      return orderList;
    }
  }
}
