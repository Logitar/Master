using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class SessionService : ISessionService
{
  private readonly ISessionClient _sessionClient;

  public SessionService(ISessionClient sessionClient)
  {
    _sessionClient = sessionClient;
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
}
