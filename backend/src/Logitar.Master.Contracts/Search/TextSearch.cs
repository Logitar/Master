namespace Logitar.Master.Contracts.Search;

/// <summary>
/// Represents textual search parameters.
/// </summary>
public record TextSearch
{
  /// <summary>
  /// Gets or sets the search terms.
  /// </summary>
  public List<SearchTerm> Terms { get; set; } = [];

  /// <summary>
  /// Gets or sets the search operator.
  /// </summary>
  public SearchOperator Operator { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="TextSearch"/> class.
  /// </summary>
  public TextSearch()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="TextSearch"/> class.
  /// </summary>
  /// <param name="terms">The search terms.</param>
  /// <param name="operator">The search operator.</param>
  public TextSearch(IEnumerable<SearchTerm> terms, SearchOperator @operator = SearchOperator.And)
  {
    Terms = terms.ToList();
    Operator = @operator;
  }
}
