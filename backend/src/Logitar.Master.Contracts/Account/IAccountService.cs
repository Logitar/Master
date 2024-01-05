using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Contracts.Account;

public interface IAccountService
{
  Task<Session> RegisterAsync(RegisterPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken = default);
}
