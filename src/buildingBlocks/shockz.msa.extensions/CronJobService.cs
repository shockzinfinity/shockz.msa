using Cronos;
using Microsoft.Extensions.Hosting;

namespace shockz.msa.extensions;

public abstract class CronJobService : IHostedService, IDisposable
{
  private System.Timers.Timer _timer;
  private readonly CronExpression _expression;
  private readonly TimeZoneInfo _timeZoneInfo;

  protected CronJobService(string expression, TimeZoneInfo timeZoneInfo)
  {
    _expression = CronExpression.Parse(expression);
    _timeZoneInfo = timeZoneInfo ?? throw new ArgumentNullException(nameof(timeZoneInfo));
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await ScheduleJob(cancellationToken);
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _timer?.Stop();
    await Task.CompletedTask;
  }

  protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
  {
    var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);

    if (next.HasValue) {
      var delay = next.Value - DateTimeOffset.Now;

      if (delay.TotalMilliseconds <= 0) {
        // timer 로 음수값이 전달되는 것을 방지
        await ScheduleJob(cancellationToken);
      }

      _timer = new(delay.TotalMilliseconds);
      _timer.Elapsed += async (sender, args) =>
      {
        _timer.Dispose(); // reset
        _timer = null;

        if (!cancellationToken.IsCancellationRequested) {
          await DoWork(cancellationToken);
        }

        if (!cancellationToken.IsCancellationRequested) {
          await ScheduleJob(cancellationToken); // reschedule next
        }
      };

      _timer.Start();
    }

    await Task.CompletedTask;
  }

  public virtual async Task DoWork(CancellationToken cancellationToken)
  {
    await Task.Delay(5000, cancellationToken);
  }

  public void Dispose()
  {
    _timer?.Dispose();
  }
}
