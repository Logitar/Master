using Logitar.Portal.Contracts.Actors;

namespace Logitar.Master.Application;

public interface IActivity
{
  void Populate(Actor actor);
}
