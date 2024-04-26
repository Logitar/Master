using Logitar.EventSourcing;
using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.Application;

public abstract record Activity
{
  public Actor Actor { get; } = new(); // TODO(fpion): resolve
  public ActorId ActorId => new(Actor.Id);
}
