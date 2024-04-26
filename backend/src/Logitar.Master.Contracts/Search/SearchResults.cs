namespace Logitar.Master.Contracts.Search;

public record SearchResults<T>
{
  public List<T> Items { get; set; }
  public long Total { get; set; }

  public SearchResults()
  {
    Items = [];
  }

  public SearchResults(IEnumerable<T> items) : this()
  {
    Items.AddRange(items);
  }

  public SearchResults(long total) : this()
  {
    Total = total;
  }

  public SearchResults(IEnumerable<T> items, long total) : this()
  {
    Items.AddRange(items);
    Total = total;
  }
}
