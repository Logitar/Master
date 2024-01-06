namespace Logitar.Master.Contracts.Sessions;

public interface ISessionService
{
  Task<Session?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<Session> RenewAsync(RenewPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken = default);
  Task<Session> SignOutAsync(string id, CancellationToken cancellationToken = default);
}
