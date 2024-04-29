using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class OneTimePasswordService : IOneTimePasswordService
{
  private const string Characters = "0123456789";
  private const int Length = 6;
  private const int LifetimeSeconds = 60 * 60;
  private const int MaximumAttempts = 5;

  private readonly IOneTimePasswordClient _oneTimePasswordClient;

  public OneTimePasswordService(IOneTimePasswordClient oneTimePasswordClient)
  {
    _oneTimePasswordClient = oneTimePasswordClient;
  }

  public async Task<OneTimePassword> CreateAsync(User user, string purpose, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = new(Characters, Length)
    {
      ExpiresOn = DateTime.Now.AddSeconds(LifetimeSeconds),
      MaximumAttempts = MaximumAttempts
    };
    payload.SetUserId(user);
    payload.SetPurpose(purpose);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _oneTimePasswordClient.CreateAsync(payload, context);
  }
}
