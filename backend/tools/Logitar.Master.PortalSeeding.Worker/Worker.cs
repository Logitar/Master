using Logitar.Master.PortalSeeding.Worker.Commands;
using Logitar.Portal.Client;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker;

internal class Worker : BackgroundService
{
  private const int MaximumAttempts = 24;
  private const int MillisecondsDelay = 5000;

  private readonly ILogger<Worker> _logger;
  private readonly IPortalSettings _portalSettings;
  private readonly IPublisher _publisher;

  public Worker(ILogger<Worker> logger, IPortalSettings portalSettings, IPublisher publisher)
  {
    _logger = logger;
    _portalSettings = portalSettings;
    _publisher = publisher;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    await WaitForStartupAsync(cancellationToken);

    await _publisher.Publish(new SeedRealmCommand(), cancellationToken);
    await _publisher.Publish(new SeedDictionariesCommand(), cancellationToken);
    await _publisher.Publish(new SeedSendersCommand(), cancellationToken);
    await _publisher.Publish(new SeedTemplatesCommand(), cancellationToken);

    chrono.Stop();
    _logger.LogInformation("Worker completed oprations after {Elapsed}ms at {Timestamp}.", chrono.ElapsedMilliseconds, DateTimeOffset.Now);
  }

  private async Task WaitForStartupAsync(CancellationToken cancellationToken)
  {
    using HttpClient client = new();
    if (_portalSettings.BaseUrl != null)
    {
      client.BaseAddress = new Uri(_portalSettings.BaseUrl, UriKind.Absolute);
    }

    for (int attempts = 0; attempts < MaximumAttempts; attempts++)
    {
      try
      {
        using HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        _ = await client.SendAsync(request, cancellationToken);
        _logger.LogInformation("Attempt {Attempt} of {Maximum} - Portal is now up and running!", attempts + 1, MaximumAttempts);

        return;
      }
      catch (Exception exception)
      {
        _logger.LogWarning("Attempt {Attempt} of {Maximum} - A '{ExceptionType}' occurred. Retrying in {Milliseconds}ms.",
          attempts + 1, MaximumAttempts, exception.GetType().Name, MillisecondsDelay);

        await Task.Delay(MillisecondsDelay, cancellationToken);
      }
    }

    throw new InvalidOperationException("The Portal could not be reached.");
  }
}
