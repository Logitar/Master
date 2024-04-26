using Logitar.Master.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Models;

public record SearchParameters
{
  protected const string DescendingKeyword = "DESC";
  protected const char SortSeparator = '.';

  [FromQuery(Name = "ids")]
  public List<Guid>? Ids { get; set; }

  [FromQuery(Name = "search_operator")]
  public SearchOperator? SearchOperator { get; set; }

  [FromQuery(Name = "search_terms")]
  public List<string>? SearchTerms { get; set; }

  [FromQuery(Name = "sort")]
  public List<string>? Sort { get; set; }

  [FromQuery(Name = "skip")]
  public int? Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int? Limit { get; set; }

  protected void Fill(SearchPayload payload)
  {
    payload.Ids = Ids;

    if (SearchTerms != null)
    {
      payload.Search = new TextSearch(SearchTerms.Select(term => new SearchTerm(term)), SearchOperator ?? default);
    }

    payload.Skip = Skip;
    payload.Limit = Limit;
  }
}
