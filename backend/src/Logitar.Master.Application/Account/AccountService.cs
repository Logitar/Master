using Logitar.Master.Application.Account.Commands;
using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Account;

internal class AccountService : IAccountService
{
  private readonly IMediator _mediator;

  public AccountService(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Session> RegisterAsync(RegisterPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new RegisterCommand(payload), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignInCommand(payload), cancellationToken);
  }
}
