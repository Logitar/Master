using FluentValidation;

namespace Logitar.Master.Domain.Shared;

public class DisplayNameValidator : AbstractValidator<string>
{
  public DisplayNameValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(DisplayNameUnit.MaximumLength);
  }
}
