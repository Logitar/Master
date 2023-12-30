using Logitar.EventSourcing;

namespace Logitar.Master.Domain.Projects;

/// <summary>
/// Represents a project identifier.
/// </summary>
public record ProjectId
{
  /// <summary>
  /// Gets the aggregate identifier.
  /// </summary>
  public AggregateId AggregateId { get; }

  internal ProjectId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  /// <summary>
  /// Parses the specified project identifier.
  /// </summary>
  /// <param name="value">The value of the identifier.</param>
  /// <returns>The parsed project identifier.</returns>
  public static ProjectId Parse(string value) => new(new AggregateId(value));
}
