using MediatR;

namespace shockz.msa.ordering.application.Features.Orders.Commands.DeleteOrder
{
  public class DeleteOrderCommand : IRequest
  {
    public int Id { get; set; }
  }
}
