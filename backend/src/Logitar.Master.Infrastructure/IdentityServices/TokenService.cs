using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class TokenService : ITokenService
{
  private readonly ITokenClient _tokenClient;

  public TokenService(ITokenClient tokenClient)
  {
    _tokenClient = tokenClient;
  }

  public async Task<CreatedToken> CreateAsync(string? subject, Email? email, string type, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = true,
      LifetimeSeconds = 3600,
      Type = type,
      Subject = subject,
    };
    if (email != null)
    {
      payload.Email = new EmailPayload(email.Address, email.IsVerified);
    }
    RequestContext context = new(cancellationToken);
    return await _tokenClient.CreateAsync(payload, context);
  }
}
