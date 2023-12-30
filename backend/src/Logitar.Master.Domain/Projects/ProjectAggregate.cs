using Logitar.EventSourcing;
using Logitar.Master.Contracts;
using Logitar.Master.Domain.Projects.Events;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// Represents a project in the Master system. A project can be seen as a collection of team members who collaborate on shared tasks.
/// </summary>
public class ProjectAggregate : AggregateRoot
{
  private ProjectUpdatedEvent _updatedEvent = new();

  private UniqueKeyUnit? _uniqueKey = null;
  /// <summary>
  /// Gets the unique key of the project.
  /// </summary>
  public UniqueKeyUnit UniqueKey => _uniqueKey ?? throw new InvalidOperationException("The unique key has not been initialized yet.");
  private DisplayNameUnit? _displayName = null;
  /// <summary>
  /// Gets or sets the display name of the project.
  /// </summary>
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
        _displayName = value;
      }
    }
  }
  private DescriptionUnit? _description = null;
  /// <summary>
  /// Gets or sets the description of the project.
  /// </summary>
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
        _description = value;
      }
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectAggregate"/> class.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  public ProjectAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectAggregate"/> class.
  /// </summary>
  /// <param name="uniqueKey">The unique key of the project.</param>
  /// <param name="actorId">The identifier of the actor who created the project.</param>
  public ProjectAggregate(UniqueKeyUnit uniqueKey, ActorId actorId) : base()
  {
    Raise(new ProjectCreatedEvent(actorId, uniqueKey));
  }
  /// <summary>
  /// Applies the specified event.
  /// </summary>
  /// <param name="event">The event to apply.</param>
  protected virtual void Apply(ProjectCreatedEvent @event)
  {
    _uniqueKey = @event.UniqueKey;
  }

  /// <summary>
  /// Deletes the project.
  /// </summary>
  /// <param name="actorId">The identifier of the actor who deleted the project.</param>
  public void Delete(ActorId actorId)
  {
    if (!IsDeleted)
    {
      Raise(new ProjectDeletedEvent(actorId));
    }
  }

  /// <summary>
  /// Changes the unique key of the project.
  /// </summary>
  /// <param name="uniqueKey">The new unique key of the project.</param>
  /// <param name="actorId">The identifier of the actor who changed the project unique key.</param>
  public void SetUniqueKey(UniqueKeyUnit uniqueKey, ActorId actorId)
  {
    if (uniqueKey != _uniqueKey)
    {
      Raise(new ProjectUniqueKeyChangedEvent(actorId, uniqueKey));
    }
  }
  /// <summary>
  /// Applies the specified event.
  /// </summary>
  /// <param name="event">The event to apply.</param>
  protected virtual void Apply(ProjectUniqueKeyChangedEvent @event)
  {
    _uniqueKey = @event.UniqueKey;
  }

  /// <summary>
  /// Applies the project update changes.
  /// </summary>
  /// <param name="actorId">The identifier of the actor who updated the project.</param>
  public void Update(ActorId actorId)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  /// <summary>
  /// Applies the specified event.
  /// </summary>
  /// <param name="event">The event to apply.</param>
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

  /// <summary>
  /// Returns a string representation of the project.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{DisplayName?.Value ?? UniqueKey.Value} | {base.ToString()}";
}
