using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Expiration;

namespace shockz.msa.fileManagement.Services;

public sealed class ExpiredFilesCleanupService : IHostedService, IDisposable
{
  private readonly ITusExpirationStore _expirationStore;
  private readonly ExpirationBase _expiration;
  private readonly ILogger<ExpiredFilesCleanupService> _logger;
  private Timer? _timer;

  public ExpiredFilesCleanupService(ILogger<ExpiredFilesCleanupService> logger, DefaultTusConfiguration config)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _expirationStore = (ITusExpirationStore)config.Store;
    _expiration = config.Expiration;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    if (_expiration == null) {
      _logger.LogInformation("Hot running cleanup job as no expiration has been set.");
      return;
    }

    await RunCleanup(cancellationToken);
    _timer = new Timer(async (e) => await RunCleanup((CancellationToken)e), cancellationToken, TimeSpan.Zero, _expiration.Timeout);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _timer?.Change(Timeout.Infinite, 0);
    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _timer?.Dispose();
  }

  private async Task RunCleanup(CancellationToken cancellationToken)
  {
    try {
      _logger.LogInformation("Running clean job...");
      var numberOfRemovedFiles = await _expirationStore.RemoveExpiredFilesAsync(cancellationToken);
      _logger.LogInformation($"Removed {numberOfRemovedFiles} expired files. Scheduled to run again in {_expiration.Timeout.TotalMilliseconds} ms"); ;
    } catch (Exception ex) {
      _logger.LogWarning("Failed to run cleanup job: " + ex.Message);
      throw;
    }
  }
}
