using FluentValidation;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Master.Contracts.Account;

namespace Logitar.Master.Application.Account.Validators;

internal class RegisterPayloadValidator : AbstractValidator<RegisterPayload>
{
  public RegisterPayloadValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));

    When(x => x.FirstName != null, () => RuleFor(x => x.FirstName!).SetValidator(new PersonNameValidator()));
    When(x => x.FirstName != null, () => RuleFor(x => x.LastName!).SetValidator(new PersonNameValidator()));
  }
}
