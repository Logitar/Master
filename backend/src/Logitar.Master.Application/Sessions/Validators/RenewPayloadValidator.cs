using FluentValidation;
using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Application.Sessions.Validators;

internal class RenewPayloadValidator : AbstractValidator<RenewPayload>
{
  public RenewPayloadValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();
  }
}
