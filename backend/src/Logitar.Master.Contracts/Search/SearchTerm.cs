namespace Logitar.Master.Contracts.Search;

/// <summary>
/// Represents a textual search term.
/// </summary>
public record SearchTerm
{
  /// <summary>
  /// Gets or sets the value of the search term.
  /// </summary>
  public string Value { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SearchTerm"/> class.
  /// </summary>
  /// <param name="value">The value of the search term.</param>
  public SearchTerm(string value)
  {
    Value = value;
  }
}
