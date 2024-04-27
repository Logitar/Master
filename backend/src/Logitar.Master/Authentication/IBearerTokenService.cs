using Logitar.Master.Models.Account;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Master.Authentication;

public interface IBearerTokenService
{
  TokenResponse GetTokenResponse(Session session);
}
