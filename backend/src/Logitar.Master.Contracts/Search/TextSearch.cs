namespace Logitar.Master.Contracts.Search;

public record TextSearch
{
  public List<SearchTerm> Terms { get; set; }
  public SearchOperator Operator { get; set; }

  public TextSearch()
  {
    Terms = [];
  }

  public TextSearch(IEnumerable<SearchTerm> terms, SearchOperator @operator = SearchOperator.And) : this()
  {
    Terms.AddRange(terms);
    Operator = @operator;
  }
}
