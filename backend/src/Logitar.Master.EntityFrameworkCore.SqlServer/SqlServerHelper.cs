using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Master.EntityFrameworkCore.Relational;

namespace Logitar.Master.EntityFrameworkCore.SqlServer;

internal class SqlServerHelper : SqlHelper
{
  public override IQueryBuilder QueryFrom(TableId table) => SqlServerQueryBuilder.From(table);
}
