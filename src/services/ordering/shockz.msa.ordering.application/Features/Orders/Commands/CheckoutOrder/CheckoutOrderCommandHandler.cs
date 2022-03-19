using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using shockz.msa.ordering.application.Contracts.Infrastructure;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.application.Models;
using shockz.msa.ordering.domain.Entities;

namespace shockz.msa.ordering.application.Features.Orders.Commands.CheckoutOrder
{
  public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
  {
    private readonly IOrderingRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommand> _logger;

    public CheckoutOrderCommandHandler(IOrderingRepository repository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommand> logger)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
      var orderEntity = _mapper.Map<Order>(request);
      var newOrder = await _repository.AddAsync(orderEntity);

      _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

      await SendEmail(newOrder);

      return newOrder.Id;
    }

    private async Task SendEmail(Order order)
    {
      var email = new Email
      {
        To = "temp@temp.com",
        Subject = "Order was created",
        Body = $"Order was created by {order.Id}"
      };

      try {
        await _emailService.SendEmail(email);
      } catch (Exception ex) {
        _logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
      }
    }
  }
}
