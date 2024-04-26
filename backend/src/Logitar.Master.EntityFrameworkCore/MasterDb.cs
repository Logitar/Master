using Logitar.Data;
using Logitar.Master.EntityFrameworkCore.Entities;

namespace Logitar.Master.EntityFrameworkCore;

internal static class MasterDb
{
  public static class Actors
  {
    public static readonly TableId Table = new(nameof(MasterContext.Actors));

    public static readonly ColumnId ActorId = new(nameof(ActorEntity.ActorId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ActorEntity.DisplayName), Table);
    public static readonly ColumnId EmailAddress = new(nameof(ActorEntity.EmailAddress), Table);
    public static readonly ColumnId Id = new(nameof(ActorEntity.Id), Table);
    public static readonly ColumnId IsDeleted = new(nameof(ActorEntity.IsDeleted), Table);
    public static readonly ColumnId PictureUrl = new(nameof(ActorEntity.PictureUrl), Table);
    public static readonly ColumnId Type = new(nameof(ActorEntity.Type), Table);
  }

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
