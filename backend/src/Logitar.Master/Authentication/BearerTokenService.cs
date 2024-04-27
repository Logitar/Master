using Logitar.Master.Extensions;
using Logitar.Master.Models.Account;
using Logitar.Master.Settings;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Master.Authentication;

internal class BearerTokenService : IBearerTokenService
{
  private const string AccessTokenType = "at+jwt";

  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly BearerTokenSettings _settings;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  private string? BaseUrl => _httpContextAccessor.HttpContext?.GetBaseUri().ToString();
  private SymmetricSecurityKey SecurityKey => new(Encoding.ASCII.GetBytes(_settings.Secret));

  public BearerTokenService(IHttpContextAccessor httpContextAccessor, BearerTokenSettings settings)
  {
    _httpContextAccessor = httpContextAccessor;
    _settings = settings;

    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  public TokenResponse GetTokenResponse(Session session)
  {
    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = BaseUrl,
      Expires = DateTime.UtcNow.AddSeconds(_settings.LifetimeSeconds),
      Issuer = BaseUrl,
      SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256),
      Subject = session.CreateClaimsIdentity(),
      TokenType = AccessTokenType
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string accessToken = _tokenHandler.WriteToken(securityToken);

    return new TokenResponse(accessToken, _settings.TokenType)
    {
      ExpiresIn = _settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
  }

  public ClaimsPrincipal ValidateToken(string token)
  {
    return _tokenHandler.ValidateToken(token, new TokenValidationParameters
    {
      IssuerSigningKey = SecurityKey,
      ValidAudience = BaseUrl,
      ValidIssuer = BaseUrl,
      ValidTypes = [AccessTokenType],
      ValidateAudience = true,
      ValidateIssuer = true,
      ValidateIssuerSigningKey = true
    }, out _);
  }
}
