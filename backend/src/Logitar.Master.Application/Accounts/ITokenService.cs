using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface ITokenService // TODO(fpion): refactor
{
  Task<CreatedToken> CreateAsync(string? subject, string type, CancellationToken cancellationToken = default);
  Task<CreatedToken> CreateAsync(string? subject, Email? email, string type, CancellationToken cancellationToken = default);
  Task<CreatedToken> CreateAsync(string? subject, IEnumerable<TokenClaim> claims, string type, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(string token, string type, CancellationToken cancellationToken = default);
  Task<ValidatedToken> ValidateAsync(string token, bool consume, string type, CancellationToken cancellationToken = default);
}
