using Logitar.Identity.Domain.Sessions;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal class SignOutCommandHandler : IRequestHandler<SignOutCommand, Session>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutCommandHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(SignOutCommand command, CancellationToken cancellationToken)
  {
    SessionId id = new(command.Id);
    SessionAggregate session = await _sessionRepository.LoadAsync(id, cancellationToken)
      ?? throw new AggregateNotFoundException<SessionAggregate>(id.AggregateId);

    session.SignOut(_applicationContext.ActorId);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
