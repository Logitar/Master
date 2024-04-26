using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Logitar.Master.EntityFrameworkCore.SqlServer;

internal class SqlServerHelper : ISqlHelper
{
  public IQueryBuilder QueryFrom(TableId table) => SqlServerQueryBuilder.From(table);
}
