using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Models.Account;

public record ResetPasswordResponse
{
  public SentMessage? RecoveryLinkSentTo { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public CurrentUser? CurrentUser { get; set; }

  public ResetPasswordResponse()
  {
  }

  public ResetPasswordResponse(ResetPasswordResult result)
  {
    RecoveryLinkSentTo = result.RecoveryLinkSentTo;
    ProfileCompletionToken = result.ProfileCompletionToken;

    if (result.Session != null)
    {
      CurrentUser = new CurrentUser(result.Session.User);
    }
  }
}
