using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The validator used to validate project unique keys.
/// </summary>
public class UniqueKeyValidator : AbstractValidator<string>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UniqueKeyValidator"/> class.
  /// </summary>
  public UniqueKeyValidator()
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(UniqueKeyUnit.MaximumLength)
      .Must(value => value.Length > 0 && !char.IsDigit(value.First()) && value.All(char.IsLetterOrDigit))
        .WithErrorCode(nameof(UniqueKeyValidator))
        .WithMessage("'{PropertyName}' may not start with a digit and may only include letters and digits.");
  }
}
