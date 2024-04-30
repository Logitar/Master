using FluentValidation;
using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Accounts.Validators;

internal class OneTimePasswordValidator : AbstractValidator<OneTimePasswordPayload>
{
  public OneTimePasswordValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Code).NotEmpty();
  }
}
