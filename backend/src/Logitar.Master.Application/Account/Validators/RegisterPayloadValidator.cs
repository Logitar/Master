using FluentValidation;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Account;

namespace Logitar.Master.Application.Account.Validators;

internal class RegisterPayloadValidator : AbstractValidator<RegisterPayload>
{
  public RegisterPayloadValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));
  }
}
