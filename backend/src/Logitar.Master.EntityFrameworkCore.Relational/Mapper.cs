using Logitar.EventSourcing;
using Logitar.Master.Contracts;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Master.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }
  public Mapper(IEnumerable<Actor> actors)
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public Project ToProject(ProjectEntity source)
  {
    Project destination = new()
    {
      Id = source.AggregateId,
      UniqueKey = source.UniqueKey,
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn;
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn;
  }

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
