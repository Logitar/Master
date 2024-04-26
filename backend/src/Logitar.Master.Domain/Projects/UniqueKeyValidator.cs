using FluentValidation;
using Logitar.Master.Domain.Shared;

namespace Logitar.Master.Domain.Projects;

public class UniqueKeyValidator : AbstractValidator<string>
{
  private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

  public UniqueKeyValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(UniqueKeyUnit.MaximumLength).SetValidator(new AllowedCharactersValidator(AllowedCharacters));
  }
}
