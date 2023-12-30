using Logitar.EventSourcing;

namespace Logitar.Master.Application;

/// <summary>
/// Defines the methods and properties of an application context.
/// </summary>
public interface IApplicationContext
{
  /// <summary>
  /// Gets the identifier of the current actor.
  /// </summary>
  ActorId ActorId { get; }
}
