using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.Contracts;

/// <summary>
/// Represents an aggregate of the system.
/// </summary>
public abstract class Aggregate
{
  /// <summary>
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// The actor who created the aggregate.
  /// </summary>
  public Actor CreatedBy { get; set; } = new();
  /// <summary>
  /// The creation date and time of the aggregate.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// The actor who updated the aggregate lastly.
  /// </summary>
  public Actor UpdatedBy { get; set; } = new();
  /// <summary>
  /// The latest update date and time of the aggregate.
  /// </summary>
  public DateTime UpdatedOn { get; set; }
}
