using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace shockz.msa.pollyPolicy;

public class PollyPolicy
{
  /// <summary>
  /// 2 ^ 1 = 2s
  /// 2 ^ 2 = 4s
  /// 2 ^ 3 = 8s
  /// 2 ^ 4 = 16s
  /// 2 ^ 5 = 32s
  /// </summary>
  /// <returns></returns>
  public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
  {
    return HttpPolicyExtensions
      .HandleTransientHttpError()
      .WaitAndRetryAsync(
        retryCount: 5,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (exception, retryCount, context) =>
        {
          Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
        });
  }

  public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
  {
    return HttpPolicyExtensions
      .HandleTransientHttpError()
      .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30));
  }
}
