using Logitar.Data;

namespace Logitar.Master.EntityFrameworkCore;

public interface ISqlHelper
{
  IQueryBuilder QueryFrom(TableId table);
}
