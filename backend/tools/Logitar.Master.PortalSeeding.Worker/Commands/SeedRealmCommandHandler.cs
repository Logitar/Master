using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal class SeedRealmCommandHandler : INotificationHandler<SeedRealmCommand>
{
  private readonly ILogger<SeedRealmCommandHandler> _logger;
  private readonly IRealmClient _realms;
  private readonly string _uniqueSlug;

  public SeedRealmCommandHandler(IConfiguration configuration, ILogger<SeedRealmCommandHandler> logger, IRealmClient realms)
  {
    _logger = logger;
    _realms = realms;
    _uniqueSlug = configuration.GetValue<string>("Portal:Realm") ?? throw new ArgumentException("The configuration 'Portal:Realm' is required.", nameof(configuration));
  }

  public async Task Handle(SeedRealmCommand _, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);

    Realm? realm = await _realms.ReadAsync(id: null, _uniqueSlug, context);
    if (realm == null)
    {
      CreateRealmPayload payload = new(_uniqueSlug, secret: string.Empty)
      {
        DisplayName = "Master",
        Description = "This is the realm of the Master project management suite.",
        DefaultLocale = "en",
        Url = "http://localhost:7791"
      };
      realm = await _realms.CreateAsync(payload, context);
      _logger.LogInformation("The realm '{UniqueSlug}' has been created (Id={Id}).", realm.UniqueSlug, realm.Id);
    }
    else
    {
      _logger.LogInformation("The realm '{UniqueSlug}' already exists (Id={Id}).", realm.UniqueSlug, realm.Id);
    }

    WorkerPortalSettings.Instance.SetRealm(realm);
  }
}
