namespace Logitar.Master.Contracts.Actors;

/// <summary>
/// The available actor types.
/// </summary>
public enum ActorType
{
  /// <summary>
  /// The actor is the system.
  /// </summary>
  System = 0,

  /// <summary>
  /// The actor is representing an API key.
  /// </summary>
  ApiKey = 1,

  /// <summary>
  /// The actor is representing an user.
  /// </summary>
  User = 2
}
