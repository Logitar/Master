﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Master.Application.Account.Validators;
using Logitar.Master.Application.Sessions;
using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Account.Commands;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Session>
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;
  private readonly IUserSettingsResolver _userSettingsResolver;

  public RegisterCommandHandler(IPasswordManager passwordManager, ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository, IUserManager userManager, IUserSettingsResolver userSettingsResolver)
  {
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
    _userSettingsResolver = userSettingsResolver;
  }

  public async Task<Session> Handle(RegisterCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = _userSettingsResolver.Resolve().UniqueName;

    RegisterPayload payload = command.Payload;
    new RegisterPayloadValidator(uniqueNameSettings).ValidateAndThrow(payload);

    UniqueNameUnit uniqueName = new(uniqueNameSettings, payload.UniqueName);
    UserId id = UserId.NewId();
    ActorId actorId = new(id.Value);

    UserAggregate user = new(uniqueName, tenantId: null, actorId, id);
    if (!string.IsNullOrWhiteSpace(payload.Password))
    {
      Password password = _passwordManager.Create(payload.Password);
      user.SetPassword(password, actorId);
    }
    user.SetEmail(EmailUnit.TryCreate(payload.EmailAddress), actorId);
    user.FirstName = PersonNameUnit.TryCreate(payload.FirstName);
    user.LastName = PersonNameUnit.TryCreate(payload.LastName);
    user.Update(actorId);

    SessionAggregate session = user.SignIn(password: null, secret: null, actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
