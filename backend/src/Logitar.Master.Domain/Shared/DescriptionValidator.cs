using FluentValidation;

namespace Logitar.Master.Domain.Shared;

public class DescriptionValidator : AbstractValidator<string>
{
  public DescriptionValidator()
  {
    RuleFor(x => x).NotEmpty();
  }
}
