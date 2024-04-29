using Logitar.Master.Application.Accounts;
using Logitar.Master.Extensions;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Master.Authentication;

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyAuthenticationHandler(IApiKeyService apiKeyService, IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _apiKeyService = apiKeyService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues values))
    {
      string? value = values.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        try
        {
          ApiKey apiKey = await _apiKeyService.AuthenticateAsync(value);

          Context.SetApiKey(apiKey);

          ClaimsPrincipal principal = new(apiKey.CreateClaimsIdentity(Scheme.Name));
          AuthenticationTicket ticket = new(principal, Scheme.Name);

          return AuthenticateResult.Success(ticket);
        }
        catch (Exception exception)
        {
          return AuthenticateResult.Fail(exception);
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
