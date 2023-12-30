using FluentValidation;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// The value object representing a project unique key.
/// </summary>
public record UniqueKeyUnit
{
  /// <summary>
  /// The maximum length of unique keys, in characters.
  /// </summary>
  public const int MaximumLength = 10;

  /// <summary>
  /// Gets the value of the unique key.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="UniqueKeyUnit"/> class.
  /// </summary>
  /// <param name="value">The value of the unique key.</param>
  public UniqueKeyUnit(string value)
  {
    Value = value.Trim();
    new UniqueKeyValidator().ValidateAndThrow(Value);
  }

  /// <summary>
  /// Creates a new instance of the <see cref="UniqueKeyUnit"/> class if the specified value is not null, empty or white-space.
  /// </summary>
  /// <param name="value">The value of the unique key.</param>
  /// <returns>The instance of the class, or null.</returns>
  public static UniqueKeyUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
