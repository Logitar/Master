using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Master.Application;

public abstract record Activity : IActivity
{
  private Actor? _actor = null;

  public Actor Actor => _actor ?? throw new InvalidOperationException($"The activity has been been populated yet. You must call the '{nameof(Populate)}' method.");
  public ActorId ActorId => new(Actor.Id);

  public void Populate(Actor actor)
  {
    if (_actor != null)
    {
      throw new InvalidOperationException($"The activity has already been populated. You may only call the '{nameof(Populate)}' method once.");
    }

    _actor = actor;
  }
}
