using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Master.Contracts.Accounts;

public record ResetPasswordResult
{
  public SentMessage? RecoveryLinkSentTo { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public Session? Session { get; set; }

  public ResetPasswordResult()
  {
  }

  public static ResetPasswordResult RecoveryLinkSent(SentMessage sentMessage) => new()
  {
    RecoveryLinkSentTo = sentMessage
  };

  public static ResetPasswordResult RequireProfileCompletion(CreatedToken createdToken) => new()
  {
    ProfileCompletionToken = createdToken.Token
  };

  public static ResetPasswordResult Succeed(Session session) => new()
  {
    Session = session
  };
}
