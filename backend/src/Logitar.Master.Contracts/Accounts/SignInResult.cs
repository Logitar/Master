using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Master.Contracts.Accounts;

public record SignInCommandResult
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public Session? Session { get; set; }

  public SignInCommandResult()
  {
  }

  public static SignInCommandResult AuthenticationLinkSent(SentMessage sentMessage) => new()
  {
    AuthenticationLinkSentTo = sentMessage
  };

  public static SignInCommandResult RequireOneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage) => new()
  {
    OneTimePasswordValidation = new OneTimePasswordValidation(oneTimePassword, sentMessage)
  };

  public static SignInCommandResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInCommandResult RequireProfileCompletion(CreatedToken createdToken) => new()
  {
    ProfileCompletionToken = createdToken.Token
  };

  public static SignInCommandResult Succeed(Session session) => new()
  {
    Session = session
  };
}
