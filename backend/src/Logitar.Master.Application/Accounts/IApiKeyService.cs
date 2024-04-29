using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Master.Application.Accounts;

public interface IApiKeyService
{
  Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken = default);
}
