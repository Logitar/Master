using Logitar.Master.Extensions;
using Logitar.Master.Models.Account;
using Logitar.Master.Settings;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Master.Authentication;

internal class BearerTokenService : IBearerTokenService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly BearerTokenSettings _settings;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public BearerTokenService(IHttpContextAccessor httpContextAccessor, BearerTokenSettings settings)
  {
    _httpContextAccessor = httpContextAccessor;
    _settings = settings;

    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  public TokenResponse GetTokenResponse(Session session)
  {
    string? baseUrl = null;
    if (_httpContextAccessor.HttpContext != null)
    {
      baseUrl = _httpContextAccessor.HttpContext.GetBaseUri().ToString();
    }

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = baseUrl,
      Expires = DateTime.UtcNow.AddSeconds(_settings.LifetimeSeconds),
      Issuer = baseUrl,
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Secret)), SecurityAlgorithms.HmacSha256),
      Subject = session.CreateClaimsIdentity(),
      TokenType = _settings.TokenType
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string accessToken = _tokenHandler.WriteToken(securityToken);

    return new TokenResponse(accessToken, _settings.TokenType)
    {
      ExpiresIn = _settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
  }
}
