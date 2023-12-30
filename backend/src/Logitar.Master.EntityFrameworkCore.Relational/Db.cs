using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Master.EntityFrameworkCore.Relational;

internal static class Db
{
  public static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
  }

  public static class Projects
  {
    public static readonly TableId Table = new(nameof(MasterContext.Projects));

    public static readonly ColumnId AggregateId = new(nameof(ProjectEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ProjectEntity.DisplayName), Table);
    public static readonly ColumnId UniqueKey = new(nameof(ProjectEntity.UniqueKey), Table);
    public static readonly ColumnId UniqueKeyNormalized = new(nameof(ProjectEntity.UniqueKeyNormalized), Table);
  }
}
