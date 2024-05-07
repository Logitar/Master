using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Accounts.Validators;

internal abstract class SaveProfileValidatorBase<T> : AbstractValidator<T> where T : SaveProfilePayload
{
  public SaveProfileValidatorBase()
  {
    RuleFor(x => x.FirstName).SetValidator(new PersonNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () => RuleFor(x => x.MiddleName!).SetValidator(new PersonNameValidator()));
    RuleFor(x => x.LastName).SetValidator(new PersonNameValidator());

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).SetValidator(new BirthdateValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).SetValidator(new GenderValidator()));
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.TimeZone).SetValidator(new TimeZoneValidator());
  }
}
