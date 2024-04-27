using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Master.Application.Accounts;

public interface ISessionService
{
  Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Session> RenewAsync(string refreshToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken = default);
}
