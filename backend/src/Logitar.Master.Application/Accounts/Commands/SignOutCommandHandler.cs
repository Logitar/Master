using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class SignOutCommandHandler : IRequestHandler<SignOutCommand, Unit>
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public SignOutCommandHandler(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  public async Task<Unit> Handle(SignOutCommand command, CancellationToken cancellationToken)
  {
    if (command.SessionId.HasValue)
    {
      _ = await _sessionService.SignOutAsync(command.SessionId.Value, cancellationToken);
    }
    else if (command.UserId.HasValue)
    {
      _ = await _userService.SignOutAsync(command.UserId.Value, cancellationToken);
    }

    return Unit.Value;
  }
}
