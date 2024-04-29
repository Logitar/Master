using Logitar.Master.Contracts.Errors;
using Logitar.Portal.Contracts.Errors;

namespace Logitar.Master.Application.Accounts;

public class OneTimePasswordNotFoundException : BadRequestException
{
  private const string ErrorMessage = "The specified One-Time Password (OTP) could not be found.";

  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }

  public override Error Error => new InvalidCredentialsError();

  public OneTimePasswordNotFoundException(Guid oneTimePasswordId) : base(BuildMessage(oneTimePasswordId))
  {
    OneTimePasswordId = oneTimePasswordId;
  }

  private static string BuildMessage(Guid oneTimePasswordId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(OneTimePasswordId), oneTimePasswordId)
    .Build();
}
