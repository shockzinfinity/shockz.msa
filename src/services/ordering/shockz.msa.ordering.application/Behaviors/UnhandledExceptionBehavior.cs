using MediatR;
using Microsoft.Extensions.Logging;
using shockz.msa.ordering.application.Exceptions;

namespace shockz.msa.ordering.application.Behaviors
{
  public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
  {
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      try {
        return await next();
      } catch (Exception ex) {
        var requestName = typeof(TRequest).Name;
        _logger.LogError(ex, "Application Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

        if (ex is ValidationException) {
          foreach (var (error, i) in (ex as ValidationException).Errors.Select((error, i) => (error, i))) {
            // NOTE: below code has performace issue.
            //_logger.LogError($"{i + 1}. {error.Key} {error.Value.Aggregate((t, x) => t + '|' + x)}");
            _logger.LogError($"{i + 1}. {error.Key}: {string.Join(" | ", error.Value)}");
          }
        }
        throw;
      }
    }
  }
}
