using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal class SeedSendersCommandHandler : INotificationHandler<SeedSendersCommand>
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<SeedSendersCommandHandler> _logger;
  private readonly ISenderClient _senders;

  public SeedSendersCommandHandler(IConfiguration configuration, ILogger<SeedSendersCommandHandler> logger, ISenderClient senders)
  {
    _configuration = configuration;
    _logger = logger;
    _senders = senders;
  }

  public async Task Handle(SeedSendersCommand _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    SearchResults<Sender> results = await _senders.SearchAsync(new SearchSendersPayload(), context);
    Dictionary<string, Sender> senders = new(capacity: results.Items.Count);
    foreach (Sender sender in results.Items)
    {
      string? key = sender.GetKey();
      if (key != null)
      {
        senders[key] = sender;
      }
    }

    IEnumerable<CreateSenderPayload> payloads = _configuration.GetSection("Senders").GetChildren()
      .Select(section => section.Get<CreateSenderPayload>() ?? new());
    foreach (CreateSenderPayload payload in payloads)
    {
      string? key = payload.GetKey();
      if (key != null)
      {
        string[] values = key.Split(':');
        if (senders.TryGetValue(key, out Sender? sender))
        {
          _logger.LogInformation("The {ContactType} sender '{ContactValue}' already exists (Id={Id}).", values[0], values[1], sender.Id);
        }
        else
        {
          sender = await _senders.CreateAsync(payload, context);
          senders[key] = sender;
          _logger.LogInformation("The {ContactType} sender '{ContactValue}' has been created (Id={Id}).", values[0], values[1], sender.Id);
        }
      }
    }
  }
}
