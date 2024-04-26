using Logitar.EventSourcing;
using Logitar.Master.Contracts;
using Logitar.Master.Domain.Projects.Events;
using Logitar.Master.Domain.Shared;

namespace Logitar.Master.Domain.Projects;

public class ProjectAggregate : AggregateRoot
{
  private ProjectUpdatedEvent _updatedEvent = new();

  public new ProjectId Id => new(base.Id);

  private UniqueKeyUnit? _uniqueKey = null;
  public UniqueKeyUnit UniqueKey => _uniqueKey ?? throw new InvalidOperationException($"The {nameof(UniqueKey)} has not been initialized yet.");
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Change<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Change<DescriptionUnit>(value);
      }
    }
  }

  public ProjectAggregate(AggregateId id) : base(id)
  {
  }

  public ProjectAggregate(UniqueKeyUnit uniqueKey, ActorId actorId = default, ProjectId? id = null) : base((id ?? ProjectId.NewId()).AggregateId)
  {
    Raise(new ProjectCreatedEvent(uniqueKey), actorId);
  }
  protected virtual void Apply(ProjectCreatedEvent @event)
  {
    _uniqueKey = @event.UniqueKey;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ProjectDeletedEvent(), actorId);
    }
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, occurredOn: DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(ProjectUpdatedEvent @event)
  {
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueKey.Value} | {base.ToString()}";
}
