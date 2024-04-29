using Logitar.EventSourcing;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.EntityFrameworkCore.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Master.EntityFrameworkCore;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors;
  private readonly Actor _system = Actor.System;

  public Mapper()
  {
    _actors = [];
  }

  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public static Actor ToActor(ActorEntity actor) => new(actor.DisplayName)
  {
    Id = new ActorId(actor.Id).ToGuid(),
    Type = actor.Type,
    IsDeleted = actor.IsDeleted,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public Project ToProject(ProjectEntity source)
  {
    Project destination = new(source.UniqueKey)
    {
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;

  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Utc => value,
    _ => throw new ArgumentException($"The date time kind '{value.Kind}' is not valid.", nameof(value)),
  };
}
