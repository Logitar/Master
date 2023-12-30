namespace Logitar.Master.Contracts.Search;

/// <summary>
/// The generic search payload.
/// </summary>
public record SearchPayload
{
  /// <summary>
  /// Gets or sets the identifier textual search parameters.
  /// </summary>
  public TextSearch Id { get; set; } = new();
  /// <summary>
  /// Gets or sets the global textual search parameters.
  /// </summary>
  public TextSearch Search { get; set; } = new();

  /// <summary>
  /// Gets or sets the sort options.
  /// </summary>
  public List<SortOption> Sort { get; set; } = [];

  /// <summary>
  /// Gets or sets the number of results to skip.
  /// </summary>
  public int Skip { get; set; }
  /// <summary>
  /// Gets or sets the number of results to return.
  /// </summary>
  public int Limit { get; set; }
}
