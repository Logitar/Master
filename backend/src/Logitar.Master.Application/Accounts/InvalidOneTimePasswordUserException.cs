using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public class InvalidOneTimePasswordUserException : InvalidCredentialsException
{
  private new const string ErrorMessage = "The specified user did not match the expected One-Time Passord (OTP) user.";

  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }
  public Guid? ExpectedUserId
  {
    get => (Guid?)Data[nameof(ExpectedUserId)];
    private set => Data[nameof(ExpectedUserId)] = value;
  }
  public Guid? ActualUserId
  {
    get => (Guid?)Data[nameof(ActualUserId)];
    private set => Data[nameof(ActualUserId)] = value;
  }

  public InvalidOneTimePasswordUserException(OneTimePassword oneTimePassword, User? user) : base(BuildMessage(oneTimePassword, user))
  {
    OneTimePasswordId = oneTimePassword.Id;
    ExpectedUserId = oneTimePassword.TryGetUserId();
    ActualUserId = user?.Id;
  }

  private static string BuildMessage(OneTimePassword oneTimePassword, User? user) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(OneTimePasswordId), oneTimePassword.Id)
    .AddData(nameof(ExpectedUserId), oneTimePassword.TryGetUserId(), "<null>")
    .AddData(nameof(ActualUserId), user?.Id, "<null>")
    .Build();
}
