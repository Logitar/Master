using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface IOneTimePasswordService
{
  Task<OneTimePassword> CreateAsync(User user, string purpose, CancellationToken cancellationToken = default);
  Task<OneTimePassword> CreateAsync(User user, string purpose, Phone? phone, CancellationToken cancellationToken = default);
  Task<OneTimePassword> ValidateAsync(OneTimePasswordPayload payload, string purpose, CancellationToken cancellationToken = default);
}
