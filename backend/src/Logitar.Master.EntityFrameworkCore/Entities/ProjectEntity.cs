using Logitar.Master.Domain.Projects.Events;

namespace Logitar.Master.EntityFrameworkCore.Entities;

internal class ProjectEntity : AggregateEntity
{
  public int ProjectId { get; private set; }

  public string UniqueKey { get; private set; } = string.Empty;
  public string UniqueKeyNormalized
  {
    get => MasterDb.Normalize(UniqueKey);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public ProjectEntity(ProjectCreatedEvent @event) : base(@event)
  {
    UniqueKey = @event.UniqueKey.Value;
  }

  private ProjectEntity() : base()
  {
  }

  public void Update(ProjectUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueKey} | {base.ToString()}";
}
