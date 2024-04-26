namespace Logitar.Master.Contracts.Search;

public record SearchPayload
{
  public List<Guid>? Ids { get; set; }
  public TextSearch? Search { get; set; }

  public List<SortOption>? Sort { get; set; }

  public int? Skip { get; set; }
  public int? Limit { get; set; }
}
