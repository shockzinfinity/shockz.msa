using MediatR;
using Microsoft.AspNetCore.Mvc;
using shockz.msa.ordering.application.Features.Orders.Commands.CheckoutOrder;
using shockz.msa.ordering.application.Features.Orders.Commands.DeleteOrder;
using shockz.msa.ordering.application.Features.Orders.Commands.UpdateOrder;
using shockz.msa.ordering.application.Features.Orders.Queries.GetOrdersList;
using System.Net;

namespace shockz.msa.ordering.api.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class OrderController : ControllerBase
  {
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{userName}", Name = "GetOrder")]
    [ProducesResponseType(typeof(IEnumerable<OrdersViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrdersViewModel>>> GetOrderByUserName(string userName)
    {
      var query = new GetOrdersListQuery(userName);
      var orders = await _mediator.Send(query);

      return Ok(orders);
    }

    // for now, test purpose
    [HttpPost(Name = "CheckoutOrder")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
    {
      var result = await _mediator.Send(command);

      return Ok(result);
    }

    [HttpPut(Name = "UpdateOrder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command)
    {
      await _mediator.Send(command);

      return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteOrder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> DeleteOrder(int id)
    {
      var command = new DeleteOrderCommand { Id = id };
      await _mediator.Send(command);

      return NoContent();
    }
  }
}
