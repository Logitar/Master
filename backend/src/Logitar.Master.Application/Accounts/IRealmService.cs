using Logitar.Portal.Contracts.Realms;

namespace Logitar.Master.Application.Accounts;

public interface IRealmService
{
  Task<Realm> FindAsync(CancellationToken cancellationToken = default);
}
