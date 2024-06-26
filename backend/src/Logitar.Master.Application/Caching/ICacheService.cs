﻿using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Master.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);
}
