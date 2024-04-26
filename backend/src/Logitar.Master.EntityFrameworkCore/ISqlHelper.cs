using Logitar.Data;
using Logitar.Master.Contracts.Search;

namespace Logitar.Master.EntityFrameworkCore;

public interface ISqlHelper
{
  void ApplyTextSearch(IQueryBuilder query, TextSearch? search, params ColumnId[] columns);
  IQueryBuilder QueryFrom(TableId table);
}
