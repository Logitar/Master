namespace Logitar.Master.Infrastructure.Settings;

/// <summary>
/// Represents caching settings.
/// </summary>
public record CacheSettings
{
  /// <summary>
  /// Gets or sets the expiration time of cached actors, in seconds.
  /// </summary>
  public int ActorExpirationSeconds { get; set; }
}
