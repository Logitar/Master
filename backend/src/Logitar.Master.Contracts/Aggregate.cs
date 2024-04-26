using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.Contracts;

public abstract class Aggregate
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is Aggregate aggregate && aggregate.GetType().Equals(GetType()) && aggregate.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{GetType()} (Id={Id})";
}
