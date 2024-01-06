using Logitar.Master.Application.Sessions.Commands;
using Logitar.Master.Application.Sessions.Queries;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions;

internal class SessionService : ISessionService
{
  private readonly IMediator _mediator;

  public SessionService(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Session?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadSessionQuery(id), cancellationToken);
  }

  public async Task<Session> RenewAsync(RenewPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new RenewCommand(payload), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignInCommand(payload), cancellationToken);
  }

  public async Task<Session> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignOutCommand(id), cancellationToken);
  }
}
