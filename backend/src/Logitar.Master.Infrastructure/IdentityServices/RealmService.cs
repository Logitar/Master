using Logitar.Master.Application.Accounts;
using Logitar.Portal.Client;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class RealmService : IRealmService
{
  private readonly IRealmClient _realmClient;
  private readonly string _uniqueSlug;

  public RealmService(IPortalSettings portalSettings, IRealmClient realmClient)
  {
    _realmClient = realmClient;
    _uniqueSlug = portalSettings.Realm ?? throw new ArgumentException($"The {nameof(portalSettings.Realm)} is required.", nameof(portalSettings));
  }

  public async Task<Realm> FindAsync(CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _realmClient.ReadAsync(id: null, _uniqueSlug, context) ?? throw new InvalidOperationException($"The realm 'UniqueSlug={_uniqueSlug}' could not be found.");
  }
}
