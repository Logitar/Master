using FluentValidation;
using Logitar.Identity.Domain.Users;
using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Accounts.Validators;

internal class CredentialsValidator : AbstractValidator<Credentials>
{
  public CredentialsValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().MaximumLength(EmailUnit.MaximumLength).EmailAddress();
    When(x => x.Password != null, () => RuleFor(x => x.Password).NotEmpty());
  }
}
