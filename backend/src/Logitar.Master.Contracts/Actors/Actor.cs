namespace Logitar.Master.Contracts.Actors;

public class Actor
{
  public string Id { get; set; } = "SYSTEM";
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public override bool Equals(object? obj) => obj is Actor actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder s = new();

    s.Append(DisplayName);

    if (EmailAddress != null)
    {
      s.Append(" <").Append(EmailAddress).Append('>');
    }

    s.Append(" (");
    if (Type != ActorType.System)
    {
      s.Append(Type).Append(".Id=");
    }
    s.Append(Id).Append(')');

    return s.ToString();
  }
}
