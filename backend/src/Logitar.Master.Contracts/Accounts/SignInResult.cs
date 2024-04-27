using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Master.Contracts.Accounts;

public record SignInCommandResult
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public Session? Session { get; set; }

  public SignInCommandResult()
  {
  }

  public static SignInCommandResult AuthenticationLinkSent(SentMessage sentMessage) => new()
  {
    AuthenticationLinkSentTo = sentMessage
  };

  public static SignInCommandResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInCommandResult Succeed(Session session) => new()
  {
    Session = session
  };
}
