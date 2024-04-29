using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.EntityFrameworkCore.Entities;

internal class ActorEntity
{
  public int ActorId { get; private set; }

  public string Id { get; private set; } = string.Empty;
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public ActorEntity(User user)
  {
    Id = new ActorId(user.Id).Value;
    Type = ActorType.User;

    Update(user);
  }

  private ActorEntity()
  {
  }

  public void Update(User user)
  {
    string id = new ActorId(user.Id).Value;
    if (Id != id || Type != ActorType.User)
    {
      throw new ArgumentException($"The actor '{Type}.Id={Id}' cannot be updated as the user 'User.Id={id}'.", nameof(user));
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object? obj) => obj is ActorEntity actor && actor.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString()
  {
    StringBuilder s = new();
    s.Append(DisplayName);
    if (EmailAddress != null)
    {
      s.Append(" <").Append(EmailAddress).Append('>');
    }
    s.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return s.ToString();
  }
}
