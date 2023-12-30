using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.EntityFrameworkCore.Relational.Entities;

internal class ActorEntity
{
  public int ActorId { get; private set; }

  public string Id { get; private set; } = string.Empty;
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  private ActorEntity()
  {
  }

  public Actor ToActor() => new()
  {
    Id = Id,
    Type = Type,
    IsDeleted = IsDeleted,
    DisplayName = DisplayName,
    EmailAddress = EmailAddress,
    PictureUrl = PictureUrl
  };
}
