using Logitar.EventSourcing;

namespace Logitar.Master.EntityFrameworkCore.Entities;

internal abstract class AggregateEntity
{
  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  protected AggregateEntity()
  {
  }

  protected AggregateEntity(DomainEvent @event)
  {
    AggregateId = @event.AggregateId.Value;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event);
  }

  public IReadOnlyCollection<ActorId> GetActorIds() => [new ActorId(CreatedBy), new ActorId(UpdatedBy)];

  protected void Update(DomainEvent @event)
  {
    Version = @event.Version;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }

  public override bool Equals(object? obj) => obj is AggregateEntity aggregate && aggregate.GetType().Equals(GetType()) && aggregate.AggregateId == AggregateId;
  public override int GetHashCode() => HashCode.Combine(GetType(), AggregateId);
  public override string ToString() => $"{GetType()} | {base.ToString()}";
}
