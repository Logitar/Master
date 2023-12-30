namespace Logitar.Master.Contracts.Actors;

/// <summary>
/// Represents an actor of the system.
/// </summary>
public class Actor
{
  /// <summary>
  /// Gets or sets the identifier of the actor.
  /// </summary>
  public string Id { get; set; } = "SYSTEM";
  /// <summary>
  /// Gets or sets the type of the actor.
  /// </summary>
  public ActorType Type { get; set; } = ActorType.System;
  /// <summary>
  /// Gets or sets a value indicating whether or not the actor is deleted.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// Gets or sets the display name of the actor.
  /// </summary>
  public string DisplayName { get; set; } = "System";
  /// <summary>
  /// Gets or sets the email address of the actor.
  /// </summary>
  public string? EmailAddress { get; set; }
  /// <summary>
  /// Gets or sets the URL of the actor's picture.
  /// </summary>
  public string? PictureUrl { get; set; }
}
