namespace Logitar.Master.Contracts.Search;

/// <summary>
/// Represents a sorting option.
/// </summary>
public record SortOption
{
  /// <summary>
  /// Gets or sets the sort field.
  /// </summary>
  public string Field { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets a value indicating whether or not the sort is descending.
  /// </summary>
  public bool IsDescending { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SortOption"/> class.
  /// </summary>
  public SortOption()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="SortOption"/> class.
  /// </summary>
  /// <param name="field">The sort field.</param>
  /// <param name="isDescending">A value indicating whether or not the sort is descending.</param>
  public SortOption(string field, bool isDescending = false)
  {
    Field = field;
    IsDescending = isDescending;
  }
}
