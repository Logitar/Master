using Logitar.Data;
using Logitar.Master.Contracts.Search;

namespace Logitar.Master.EntityFrameworkCore;

public abstract class SqlHelper : ISqlHelper
{
  public virtual void ApplyTextSearch(IQueryBuilder query, TextSearch? search, params ColumnId[] columns)
  {
    if (search != null && search.Terms.Count > 0 && columns.Length > 0)
    {
      List<Condition> conditions = new(capacity: search.Terms.Count);
      HashSet<string> patterns = new(capacity: search.Terms.Count);
      foreach (SearchTerm term in search.Terms)
      {
        if (!string.IsNullOrWhiteSpace(term.Value))
        {
          string pattern = term.Value.Trim();
          if (patterns.Add(pattern))
          {
            conditions.Add(columns.Length == 1
              ? new OperatorCondition(columns[0], CreateOperator(pattern))
              : new OrCondition(columns.Select(column => new OperatorCondition(column, CreateOperator(pattern))).ToArray()));
          }
        }
      }

      if (conditions.Count > 0)
      {
        switch (search.Operator)
        {
          case SearchOperator.And:
            query.WhereAnd([.. conditions]);
            break;
          case SearchOperator.Or:
            query.WhereOr([.. conditions]);
            break;
        }
      }
    }
  }
  public virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);

  public abstract IQueryBuilder QueryFrom(TableId table);
}
