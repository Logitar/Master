using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class UserService : IUserService
{
  private readonly IUserClient _userClient;

  public UserService(IUserClient userClient)
  {
    _userClient = userClient;
  }

  public async Task<User> AuthenticateAsync(User user, string password, CancellationToken cancellationToken)
  {
    return await AuthenticateAsync(user.UniqueName, password, cancellationToken);
  }
  public async Task<User> AuthenticateAsync(string uniqueName, string password, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = new(uniqueName, password);
    RequestContext context = new(uniqueName, cancellationToken);
    return await _userClient.AuthenticateAsync(payload, context);
  }

  public async Task<User?> FindAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, uniqueName, identifier: null, context);
  }

  public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id, uniqueName: null, identifier: null, context);
  }
}
