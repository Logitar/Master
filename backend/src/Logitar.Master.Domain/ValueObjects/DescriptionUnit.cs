using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The value object representing a textual description.
/// </summary>
public record DescriptionUnit
{
  /// <summary>
  /// Gets the value of the description.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DescriptionUnit"/> class.
  /// </summary>
  /// <param name="value">The value of the description.</param>
  public DescriptionUnit(string value)
  {
    Value = value.Trim();
    new DescriptionValidator().ValidateAndThrow(Value);
  }
}
