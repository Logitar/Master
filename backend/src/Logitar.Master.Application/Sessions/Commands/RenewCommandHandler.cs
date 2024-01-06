using FluentValidation;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Master.Application.Sessions.Validators;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal class RenewCommandHandler : IRequestHandler<RenewCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RenewCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(RenewCommand command, CancellationToken cancellationToken)
  {
    RenewPayload payload = command.Payload;
    new RenewPayloadValidator().ValidateAndThrow(payload);

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(payload.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, innerException);
    }

    SessionId id = new(refreshToken.Id);
    SessionAggregate session = await _sessionRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<SessionAggregate>(id.AggregateId);

    Password newSecret = _passwordManager.Generate(RefreshToken.SecretLength, out byte[] secretBytes);

    session.Renew(refreshToken.Secret, newSecret, _applicationContext.ActorId);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    result.RefreshToken = new RefreshToken(result.Id, secretBytes).Encode();

    return result;
  }
}
