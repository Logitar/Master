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

  /// <summary>
  /// Creates a new instance of the <see cref="DescriptionUnit"/> class if the specified value is not null, empty or white-space.
  /// </summary>
  /// <param name="value">The value of the description.</param>
  /// <returns>The instance of the class, or null.</returns>
  public static DescriptionUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
