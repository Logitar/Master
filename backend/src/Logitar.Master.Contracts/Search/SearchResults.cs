namespace Logitar.Master.Contracts.Search;

/// <summary>
/// Represents a page of search results.
/// </summary>
/// <typeparam name="T">The result type.</typeparam>
public record SearchResults<T>
{
  /// <summary>
  /// Gets or sets the result items.
  /// </summary>
  public List<T> Items { get; set; } = [];
  /// <summary>
  /// Gets or sets the total result count.
  /// </summary>
  public long Total { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SearchResults{T}"/> class.
  /// </summary>
  public SearchResults()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="SearchResults{T}"/> class.
  /// </summary>
  /// <param name="items">The result items.</param>
  public SearchResults(IEnumerable<T> items) : this(items, items.LongCount())
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="SearchResults{T}"/> class.
  /// </summary>
  /// <param name="total">The total result count.</param>
  public SearchResults(long total)
  {
    Total = total;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="SearchResults{T}"/> class.
  /// </summary>
  /// <param name="items">The result items.</param>
  /// <param name="total">The total result count.</param>
  public SearchResults(IEnumerable<T> items, long total)
  {
    Items = items.ToList();
    Total = total;
  }
}
