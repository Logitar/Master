using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Accounts.Validators;

internal class SignInValidator : AbstractValidator<SignInPayload>
{
  public SignInValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());

    When(x => x.Credentials != null, () => RuleFor(x => x.Credentials!).SetValidator(new CredentialsValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));
    When(x => x.Profile != null, () => RuleFor(x => x.Profile!).SetValidator(new CompleteProfileValidator(passwordSettings)));

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(SignInValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.Credentials)}, {nameof(x.AuthenticationToken)}, {nameof(x.OneTimePassword)}, {nameof(x.Profile)}.");
  }

  private static bool BeAValidPayload(SignInPayload payload)
  {
    int count = 0;
    if (payload.Credentials != null)
    {
      count++;
    }
    if (payload.AuthenticationToken != null)
    {
      count++;
    }
    if (payload.OneTimePassword != null)
    {
      count++;
    }
    if (payload.Profile != null)
    {
      count++;
    }
    return count == 1;
  }
}
