using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class SessionService : ISessionService
{
  private readonly ISessionClient _sessionClient;

  public SessionService(ISessionClient sessionClient)
  {
    _sessionClient = sessionClient;
  }

  public async Task<Session> CreateAsync(User user, IEnumerable<CustomAttribute>? customAttributes, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = new(user.Id.ToString(), isPersistent: true, customAttributes ?? []);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _sessionClient.CreateAsync(payload, context);
  }

  public async Task<Session?> FindAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _sessionClient.ReadAsync(id, context);
  }

  public async Task<Session> RenewAsync(string refreshToken, IEnumerable<CustomAttribute>? customAttributes, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = new(refreshToken, customAttributes ?? []);
    RequestContext context = new(cancellationToken);
    return await _sessionClient.RenewAsync(payload, context);
  }

  public async Task<Session> SignInAsync(User user, string password, IEnumerable<CustomAttribute>? customAttributes, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(user.UniqueName, password, isPersistent: true, customAttributes ?? []);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _sessionClient.SignInAsync(payload, context);
  }

  public async Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _sessionClient.SignOutAsync(id, context);
  }
}
