using Logitar.Master.Application.Accounts;
using Logitar.Master.Application.Constants;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using System.Security.Claims;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class TokenService : ITokenService
{
  private readonly ITokenClient _tokenClient;

  public TokenService(ITokenClient tokenClient)
  {
    _tokenClient = tokenClient;
  }

  public async Task<CreatedToken> CreateAsync(string? subject, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(subject, email: null, type, cancellationToken);
  }
  public async Task<CreatedToken> CreateAsync(string? subject, Email? email, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(subject, email, phone: null, type, cancellationToken);
  }
  public async Task<CreatedToken> CreateAsync(string? subject, Phone? phone, string type, CancellationToken cancellationToken)
  {
    return await CreateAsync(subject, email: null, phone, type, cancellationToken);
  }
  private async Task<CreatedToken> CreateAsync(string? subject, Email? email, Phone? phone, string type, CancellationToken cancellationToken)
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
    if (phone != null)
    {
      if (phone.CountryCode != null)
      {
        payload.Claims.Add(new TokenClaim(ClaimNames.PhoneCountryCode, phone.CountryCode));
      }
      payload.Claims.Add(new TokenClaim(ClaimNames.PhoneNumberRaw, phone.Number));
      string phoneNumber = phone.Extension == null ? phone.E164Formatted : $"{phone.E164Formatted};ext={phone.Extension}";
      payload.Claims.Add(new TokenClaim(Rfc7519ClaimNames.PhoneNumber, phoneNumber));
      payload.Claims.Add(new TokenClaim(Rfc7519ClaimNames.IsPhoneVerified, phone.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    RequestContext context = new(cancellationToken);
    return await _tokenClient.CreateAsync(payload, context);
  }

  public async Task<ValidatedToken> ValidateAsync(string token, string type, CancellationToken cancellationToken)
  {
    return await ValidateAsync(token, consume: true, type, cancellationToken);
  }
  public async Task<ValidatedToken> ValidateAsync(string token, bool consume, string type, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = new(token)
    {
      Consume = consume,
      Type = type
    };
    RequestContext context = new(cancellationToken);
    return await _tokenClient.ValidateAsync(payload, context);
  }
}
