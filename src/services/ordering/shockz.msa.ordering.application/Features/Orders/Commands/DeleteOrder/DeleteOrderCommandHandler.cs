using MediatR;
using Microsoft.Extensions.Logging;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.application.Exceptions;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.application.Features.Orders.Commands.DeleteOrder
{
  public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
  {
    private readonly IOrderingRepository _repository;
    private readonly ILogger<DeleteOrderCommandHandler> _logger;

    public DeleteOrderCommandHandler(IOrderingRepository repository, ILogger<DeleteOrderCommandHandler> logger)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
      var orderToDelete = await _repository.GetByIdAsync(request.Id);
      if (orderToDelete == null) {
        //_logger.LogError("Order not exist on database.");
        throw new NotFoundException(nameof(Order), request.Id);
      }

      await _repository.DeleteAsync(orderToDelete);
      _logger.LogInformation($"Order {orderToDelete.Id} is successfully deleted.");

      return Unit.Value;
    }
  }
}
