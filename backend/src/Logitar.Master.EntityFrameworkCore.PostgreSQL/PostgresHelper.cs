using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Master.EntityFrameworkCore.Relational;

namespace Logitar.Master.EntityFrameworkCore.PostgreSQL;

internal class PostgresHelper : SqlHelper
{
  public override IQueryBuilder QueryFrom(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
