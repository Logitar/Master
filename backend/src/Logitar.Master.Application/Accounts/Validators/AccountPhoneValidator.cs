using FluentValidation;
using Logitar.Identity.Domain.Users;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts.Validators;

internal class AccountPhoneValidator : AbstractValidator<AccountPhone>
{
  public AccountPhoneValidator()
  {
    When(x => x.CountryCode != null, () => RuleFor(x => x.CountryCode).NotEmpty().Length(2));
    RuleFor(x => x.Number).NotEmpty().MaximumLength(20);

    RuleFor(x => x).Must(BeAValidPhone).WithErrorCode("PhoneValidator").WithMessage("'{PropertyName}' must be a valid phone.");
  }

  private static bool BeAValidPhone(AccountPhone input)
  {
    Phone phone = new(input.CountryCode, input.Number, extension: null, e164Formatted: string.Empty);
    return phone.IsValid();
  }
}
