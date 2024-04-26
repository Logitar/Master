using FluentValidation;

namespace Logitar.Master.Domain.Projects;

public class UniqueKeyValidator : AbstractValidator<string>
{
  public UniqueKeyValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(UniqueKeyUnit.MaximumLength); // TODO(fpion): AllowedCharacters
  }
}
