using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface IUserService
{
  Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
  Task<User?> FindAsync(string emailAddress, CancellationToken cancellationToken = default);
  Task<User?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
