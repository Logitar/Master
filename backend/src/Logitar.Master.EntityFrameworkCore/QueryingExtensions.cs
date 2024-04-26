using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Master.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder query, ColumnId column, IEnumerable<Guid>? ids)
  {
    if (ids != null && ids.Any())
    {
      string[] aggregateIds = ids.Distinct().Select(id => new AggregateId(id).Value).ToArray();
      query.Where(column, Operators.IsIn(aggregateIds));
    }

    return query;
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    if (payload.Skip > 0)
    {
      query = query.Skip(payload.Skip.Value);
    }

    if (payload.Limit > 0)
    {
      query = query.Take(payload.Limit.Value);
    }

    return query;
  }

  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQueryBuilder query) where T : class
  {
    return entities.FromQuery(query.Build());
  }
  public static IQueryable<T> FromQuery<T>(this DbSet<T> entities, IQuery query) where T : class
  {
    return entities.FromSqlRaw(query.Text, query.Parameters.ToArray());
  }
}
