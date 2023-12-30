using Logitar.Master.Domain.Projects.Events;

namespace Logitar.Master.EntityFrameworkCore.Relational.Entities;

internal class ProjectEntity : AggregateEntity
{
  public int ProjectId { get; private set; }

  public string UniqueKey { get; private set; } = string.Empty;
  public string UniqueKeyNormalized
  {
    get => UniqueKey.ToUpper();
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

  public void SetUniqueKey(ProjectUniqueKeyChangedEvent @event)
  {
    Update(@event);

    UniqueKey = @event.UniqueKey.Value;
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
}
