using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application;

public abstract record Activity : IActivity
{
  private readonly Actor _system = Actor.System;
  private ActivityContext? _context = null;

  public Actor Actor
  {
    get
    {
      if (_context == null)
      {
        throw new InvalidOperationException($"The activity has been been contextualized yet. You must call the '{nameof(Contextualize)}' method.");
      }

      if (_context.User != null)
      {
        return new Actor(_context.User);
      }

      if (_context.ApiKey != null)
      {
        return new Actor(_context.ApiKey);
      }

      return _system;
    }
  }
  public ActorId ActorId => new(Actor.Id);

  public User? User
  {
    get
    {
      if (_context == null)
      {
        throw new InvalidOperationException($"The activity has been been contextualized yet. You must call the '{nameof(Contextualize)}' method.");
      }

      return _context.User;
    }
  }

  public void Contextualize(ActivityContext context)
  {
    if (_context != null)
    {
      throw new InvalidOperationException($"The activity has already been populated. You may only call the '{nameof(Contextualize)}' method once.");
    }

    _context = context;
  }
}
