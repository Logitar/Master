using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Accounts.Validators;

internal class VerifyPhoneValidator : AbstractValidator<VerifyPhonePayload>
{
  public VerifyPhoneValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.ProfileCompletionToken).NotEmpty();

    When(x => x.NewPhone != null, () => RuleFor(x => x.NewPhone!).SetValidator(new AccountPhoneValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(VerifyPhoneValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.NewPhone)}, {nameof(x.OneTimePassword)}.");
  }

  private static bool BeAValidPayload(VerifyPhonePayload payload)
  {
    int count = 0;
    if (payload.NewPhone != null)
    {
      count++;
    }
    if (payload.OneTimePassword != null)
    {
      count++;
    }
    return count == 1;
  }
}
