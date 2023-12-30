using Logitar.EventSourcing;
using Logitar.Master.Application;

namespace Logitar.Master;

internal class HttpApplicationContext : IApplicationContext
{
  public ActorId ActorId { get; } // TODO(fpion): Authentication
}
