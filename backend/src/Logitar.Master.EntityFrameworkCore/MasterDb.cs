using Logitar.Data;
using Logitar.Master.EntityFrameworkCore.Entities;

namespace Logitar.Master.EntityFrameworkCore;

internal static class MasterDb
{
  public static class Projects
  {
    public static readonly TableId Table = new(nameof(MasterContext.Projects));

    public static readonly ColumnId AggregateId = new(nameof(ProjectEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ProjectEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ProjectEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ProjectEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ProjectEntity.DisplayName), Table);
    public static readonly ColumnId ProjectId = new(nameof(ProjectEntity.ProjectId), Table);
    public static readonly ColumnId UniqueKey = new(nameof(ProjectEntity.UniqueKey), Table);
    public static readonly ColumnId UniqueKeyNormalized = new(nameof(ProjectEntity.UniqueKeyNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ProjectEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ProjectEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ProjectEntity.Version), Table);
  }

  public static string Normalize(string value) => value.Trim().ToUpper();
}
