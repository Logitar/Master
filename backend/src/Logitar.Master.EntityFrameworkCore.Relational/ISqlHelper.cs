using Logitar.Data;
using Logitar.Master.Contracts.Search;

namespace Logitar.Master.EntityFrameworkCore.Relational;

/// <summary>
/// Define methods to execute SQL commands.
/// </summary>
public interface ISqlHelper
{
  /// <summary>
  /// Applies the specified textual search to the specified columns in the specified query builder.
  /// </summary>
  /// <param name="builder">The query builder.</param>
  /// <param name="search">The textual search.</param>
  /// <param name="columns">The search columns.</param>
  /// <returns>The query builder.</returns>
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);
  /// <summary>
  /// Queries the specified data table.
  /// </summary>
  /// <param name="table">The data table.</param>
  /// <returns>The query builder.</returns>
  IQueryBuilder QueryFrom(TableId table);
}
