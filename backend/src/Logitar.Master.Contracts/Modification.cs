namespace Logitar.Master.Contracts;

/// <summary>
/// The value object representing a modified nullable value.
/// </summary>
/// <typeparam name="T">The type of the modified value.</typeparam>
public record Modification<T>
{
  /// <summary>
  /// Gets the value of the modification.
  /// </summary>
  public T? Value { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Modification{T}"/> class.
  /// </summary>
  /// <param name="value">The value of the modification.</param>
  public Modification(T? value)
  {
    Value = value;
  }
}
