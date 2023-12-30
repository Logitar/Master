using Logitar.Data;
using Logitar.Master.Contracts.Search;

namespace Logitar.Master.EntityFrameworkCore.Relational;

/// <summary>
/// Represents an abstraction of a helper class for SQL commands.
/// </summary>
public abstract class SqlHelper : ISqlHelper
{
  /// <summary>
  /// Applies the specified textual search to the specified columns in the specified query builder.
  /// </summary>
  /// <param name="builder">The query builder.</param>
  /// <param name="search">The textual search.</param>
  /// <param name="columns">The search columns.</param>
  /// <returns>The query builder.</returns>
  public IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns)
  {
    if (search.Terms.Count == 0 || columns.Length == 0)
    {
      return builder;
    }

    List<Condition> conditions = new(capacity: search.Terms.Count);
    foreach (SearchTerm term in search.Terms)
    {
      if (!string.IsNullOrWhiteSpace(term.Value))
      {
        string pattern = term.Value.Trim();
        conditions.Add(columns.Length == 1
          ? new OperatorCondition(columns.Single(), CreateOperator(pattern))
          : new OrCondition(columns.Select(column => new OperatorCondition(column, CreateOperator(pattern))).ToArray()));
      }
    }

    if (conditions.Count > 0)
    {
      switch (search.Operator)
      {
        case SearchOperator.And:
          return builder.WhereAnd([.. conditions]);
        case SearchOperator.Or:
          return builder.WhereOr([.. conditions]);
      }
    }

    return builder;
  }
  /// <summary>
  /// Creates a conditional operator for the specified pattern.
  /// </summary>
  /// <param name="pattern">The search pattern.</param>
  /// <returns>The conditional operator.</returns>
  protected virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);

  /// <summary>
  /// Queries the specified data table.
  /// </summary>
  /// <param name="table">The data table.</param>
  /// <returns>The query builder.</returns>
  public abstract IQueryBuilder QueryFrom(TableId table);
}
