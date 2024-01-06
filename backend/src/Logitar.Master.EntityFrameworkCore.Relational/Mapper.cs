using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Master.Contracts;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.Contracts.Users;

namespace Logitar.Master.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<Actor> actors)
  {
    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public virtual Actor ToActor(ActorEntity actor) => new()
  {
    Id = actor.Id,
    Type = Enum.Parse<ActorType>(actor.Type),
    IsDeleted = actor.IsDeleted,
    DisplayName = actor.DisplayName,
    EmailAddress = actor.EmailAddress,
    PictureUrl = actor.PictureUrl
  };

  public Session ToSession(SessionEntity source) => ToSession(source, user: null);
  public Session ToSession(SessionEntity source, User? user)
  {
    Session destination = new()
    {
      IsPersistent = source.IsPersistent,
      SignedOutBy = TryGetActor(source.SignedOutBy),
      SignedOutOn = AsUniversalTime(source.SignedOutOn),
      IsActive = source.IsActive
    };
    destination.User = user ?? (source.User == null ? null : ToUser(source.User, [destination]));

    MapAggregate(source, destination);

    return destination;
  }

  public User ToUser(UserEntity source) => ToUser(source, sessions: null);
  public User ToUser(UserEntity source, IEnumerable<Session>? sessions)
  {
    User destination = new()
    {
      TenantId = source.TenantId,
      UniqueName = source.UniqueName,
      PasswordChangedBy = TryGetActor(source.PasswordChangedBy),
      PasswordChangedOn = AsUniversalTime(source.PasswordChangedOn),
      HasPassword = source.HasPassword,
      DisabledBy = TryGetActor(source.DisabledBy),
      DisabledOn = AsUniversalTime(source.DisabledOn),
      IsDisabled = source.IsDisabled,
      IsConfirmed = source.IsConfirmed,
      FirstName = source.FirstName,
      MiddleName = source.MiddleName,
      LastName = source.LastName,
      FullName = source.FullName,
      Nickname = source.Nickname,
      Birthdate = source.Birthdate,
      Gender = source.Gender,
      Locale = source.Locale,
      TimeZone = source.TimeZone,
      Picture = source.Picture,
      Profile = source.Profile,
      Website = source.Website,
      AuthenticatedOn = source.AuthenticatedOn
    };
    destination.Sessions = (sessions ?? source.Sessions.Select(session => ToSession(session, destination))).ToList();

    if (source.EmailAddress != null)
    {
      destination.Email = new Email
      {
        VerifiedBy = TryGetActor(source.EmailVerifiedBy),
        VerifiedOn = AsUniversalTime(source.EmailVerifiedOn),
        IsVerified = source.IsEmailVerified,
        Address = source.EmailAddress
      };
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = source.AggregateId;
    destination.Version = source.Version;
    destination.CreatedBy = GetActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = GetActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor GetActor(string id) => GetActor(new ActorId(id));
  private Actor GetActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
  private Actor? TryGetActor(string? id) => id == null ? null : GetActor(id);

  private static DateTime AsUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
  private static DateTime? AsUniversalTime(DateTime? value) => value.HasValue ? AsUniversalTime(value.Value) : null;
}
