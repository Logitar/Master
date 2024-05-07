using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

public record SignOutCommand : IRequest<Unit>
{
  public Guid? SessionId { get; }
  public Guid? UserId { get; }

  public SignOutCommand(Session session)
  {
    SessionId = session.Id;
  }
  public SignOutCommand(User user)
  {
    UserId = user.Id;
  }
}
