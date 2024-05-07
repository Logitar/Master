using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Models.Account;

public record SignInResponse
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public CurrentUser? CurrentUser { get; set; }

  public SignInResponse()
  {
  }

  public SignInResponse(SignInCommandResult result)
  {
    AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
    IsPasswordRequired = result.IsPasswordRequired;
    OneTimePasswordValidation = result.OneTimePasswordValidation;
    ProfileCompletionToken = result.ProfileCompletionToken;

    if (result.Session != null)
    {
      CurrentUser = new CurrentUser(result.Session.User);
    }
  }
}
