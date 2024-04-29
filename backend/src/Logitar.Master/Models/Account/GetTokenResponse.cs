using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Models.Account;

public record GetTokenResponse
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public TokenResponse? TokenResponse { get; set; }

  public GetTokenResponse()
  {
  }

  public GetTokenResponse(SignInCommandResult result)
  {
    AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
    IsPasswordRequired = result.IsPasswordRequired;
  }
}
