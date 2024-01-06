using FluentValidation;
using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Application.Sessions.Validators;

internal class SignInPayloadValidator : AbstractValidator<SignInPayload>
{
  public SignInPayloadValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
