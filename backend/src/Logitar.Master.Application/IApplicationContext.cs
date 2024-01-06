using Logitar.EventSourcing;

namespace Logitar.Master.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }
}
