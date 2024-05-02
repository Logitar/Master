using Logitar.Master.PortalSeeding.Worker.Commands;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker;

internal class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;
  private readonly IPublisher _publisher;

  public Worker(ILogger<Worker> logger, IPublisher publisher)
  {
    _logger = logger;
    _publisher = publisher;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    await _publisher.Publish(new SeedRealmCommand(), cancellationToken);
    await _publisher.Publish(new SeedDictionariesCommand(), cancellationToken);
    await _publisher.Publish(new SeedSendersCommand(), cancellationToken);
    await _publisher.Publish(new SeedTemplatesCommand(), cancellationToken);

    chrono.Stop();
    _logger.LogInformation("Worker completed oprations after {Elapsed}ms at {Timestamp}.", chrono.ElapsedMilliseconds, DateTimeOffset.Now);
  }
}
