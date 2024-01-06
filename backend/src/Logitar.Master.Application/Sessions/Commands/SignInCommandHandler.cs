using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Master.Application.Sessions.Validators;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, Session>
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public SignInCommandHandler(IPasswordManager passwordManager, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserManager userManager)
  {
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<Session> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    SignInPayload payload = command.Payload;
    new SignInPayloadValidator().ValidateAndThrow(payload);

    UserAggregate user = await _userManager.FindAsync(payload.TenantId, payload.UniqueName, cancellationToken)
      ?? throw new UserNotFoundException(payload.TenantId, payload.UniqueName);

    ActorId actorId = new(user.Id.Value);
    Password? secret = null;
    byte[]? secretBytes = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.Generate(RefreshToken.SecretLength, out secretBytes);
    }
    SessionAggregate session = user.SignIn(payload.Password, secret, actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretBytes != null)
    {
      result.RefreshToken = new RefreshToken(result.Id, secretBytes).Encode();
    }

    return result;
  }
}
