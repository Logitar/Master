using Logitar.Master.Models.Account;
using Logitar.Master.Settings;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Master.Authentication;

internal class BearerTokenService : IBearerTokenService
{
  private readonly BearerTokenSettings _settings;

  public BearerTokenService(BearerTokenSettings settings)
  {
    _settings = settings;
  }

  public TokenResponse GetTokenResponse(Session session)
  {
    string accessToken = ""; // TODO(fpion): implement

    return new TokenResponse(accessToken, _settings.TokenType)
    {
      ExpiresIn = _settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
  }
}
