using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The value object representing a display name.
/// </summary>
public record DisplayNameUnit
{
  /// <summary>
  /// The maximum length of display names, in characters.
  /// </summary>
  public const int MaximumLength = 50;

  /// <summary>
  /// Gets the value of the display name.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DisplayNameUnit"/> class.
  /// </summary>
  /// <param name="value">The value of the display name.</param>
  public DisplayNameUnit(string value)
  {
    Value = value.Trim();
    new DisplayNameValidator().ValidateAndThrow(Value);
  }

  /// <summary>
  /// Creates a new instance of the <see cref="DisplayNameUnit"/> class if the specified value is not null, empty or white-space.
  /// </summary>
  /// <param name="value">The value of the display name.</param>
  /// <returns>The instance of the class, or null.</returns>
  public static DisplayNameUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
