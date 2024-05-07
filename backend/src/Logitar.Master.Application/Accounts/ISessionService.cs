using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface ISessionService
{
  Task<Session> CreateAsync(User user, IEnumerable<CustomAttribute>? customAttributes = null, CancellationToken cancellationToken = default);
  Task<Session?> FindAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Session> RenewAsync(string refreshToken, IEnumerable<CustomAttribute>? customAttributes = null, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(User user, string password, IEnumerable<CustomAttribute>? customAttributes = null, CancellationToken cancellationToken = default);
  Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken = default);
}
