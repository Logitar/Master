using FluentValidation;

namespace Logitar.Master.Domain.Shared;

public class AllowedCharactersValidator : AbstractValidator<string>
{
  public AllowedCharactersValidator(string? allowedCharacters)
  {
    if (allowedCharacters != null)
    {
      RuleFor(x => x).Must(x => x.All(allowedCharacters.Contains)).WithErrorCode(nameof(AllowedCharactersValidator))
        .WithMessage($"'{{PropertyName}}' may only contain the following characters: '{allowedCharacters}'.");
    }
  }
}
