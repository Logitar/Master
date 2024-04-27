using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class ApiKeyService : IApiKeyService
{
  private readonly IApiKeyClient _apiKeyClient;

  public ApiKeyService(IApiKeyClient apiKeyClient)
  {
    _apiKeyClient = apiKeyClient;
  }

  public async Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = new(xApiKey);
    RequestContext context = new(cancellationToken);
    return await _apiKeyClient.AuthenticateAsync(payload, context);
  }
}
