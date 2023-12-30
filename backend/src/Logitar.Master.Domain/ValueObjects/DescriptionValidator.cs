using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The validator used to validate descriptions.
/// </summary>
public class DescriptionValidator : AbstractValidator<string>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DisplayNameValidator"/> class.
  /// </summary>
  public DescriptionValidator()
  {
    RuleFor(x => x).NotEmpty();
  }
}
