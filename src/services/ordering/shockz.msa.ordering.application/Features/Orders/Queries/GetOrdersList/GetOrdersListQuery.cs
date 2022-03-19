using MediatR;

namespace shockz.msa.ordering.application.Features.Orders.Queries.GetOrdersList
{
  public class GetOrdersListQuery : IRequest<List<OrdersViewModel>>
  {
    public string UserName { get; set; }

    public GetOrdersListQuery(string userName)
    {
      UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
  }
}
