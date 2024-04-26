using Logitar.EventSourcing;

namespace Logitar.Master.Domain.Projects;

public record ProjectId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ProjectId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private ProjectId(string value)
  {
    AggregateId = new(value.Trim());
  }

  public static ProjectId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static ProjectId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new ProjectId(value.Trim());
  }
}
