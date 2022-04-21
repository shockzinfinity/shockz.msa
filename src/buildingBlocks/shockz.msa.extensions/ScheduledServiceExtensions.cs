using Microsoft.Extensions.DependencyInjection;

namespace shockz.msa.extensions;

public static class ScheduledServiceExtensions
{
  public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
  {
    if (options == null) throw new ArgumentNullException(nameof(options), @"Please provide schedule configuration.");

    var config = new ScheduleConfig<T>();
    options.Invoke(config);

    if (string.IsNullOrWhiteSpace(config.CronExpression)) {
      throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty cron expression is not allowed.");
    }

    services.AddSingleton<IScheduleConfig<T>>(config);
    services.AddHostedService<T>();

    return services;
  }
}
