using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface IUserService
{
  Task<User?> FindAsync(string emailAddress, CancellationToken cancellationToken = default);
}
