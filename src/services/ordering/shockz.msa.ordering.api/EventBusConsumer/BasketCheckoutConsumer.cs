﻿using AutoMapper;
using MassTransit;
using shockz.msa.eventBus.messages.Events;
using shockz.msa.ordering.application.Features.Orders.Commands.CheckoutOrder;
using IMediator = MediatR.IMediator;

namespace shockz.msa.ordering.api.EventBusConsumer
{
  public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
  {
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<BasketCheckoutConsumer> _logger;

    public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger<BasketCheckoutConsumer> logger)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
      var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
      var result = await _mediator.Send(command);

      _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id: {newOrderId}", result);
    }
  }
}
