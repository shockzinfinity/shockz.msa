using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.application.Exceptions;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.application.Features.Orders.Commands.UpdateOrder
{
  public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
  {
    private readonly IOrderingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(IOrderingRepository repository, IMapper mapper, ILogger<UpdateOrderCommandHandler> logger)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // unit type means void type
    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
      var orderToUpdate = await _repository.GetByIdAsync(request.Id);
      if (orderToUpdate == null) {
        //_logger.LogError("Order not exist on database.");
        throw new NotFoundException(nameof(Order), request.Id);
      }

      // explict mapping
      _mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));

      await _repository.UpdateAsync(orderToUpdate);

      _logger.LogInformation($"Order {orderToUpdate.Id} is successfully updated.");

      return Unit.Value;
    }
  }
}
