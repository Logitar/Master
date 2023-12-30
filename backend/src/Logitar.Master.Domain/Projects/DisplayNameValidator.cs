using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The validator used to validate display names.
/// </summary>
public class DisplayNameValidator : AbstractValidator<string>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DisplayNameValidator"/> class.
  /// </summary>
  public DisplayNameValidator()
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(DisplayNameUnit.MaximumLength);
  }
}
