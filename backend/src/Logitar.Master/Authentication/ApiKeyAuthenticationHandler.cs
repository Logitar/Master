using Logitar.Portal.Contracts.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Master.Authentication;

internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
  public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
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
          await Task.Delay(1000); // TODO(fpion): implement
          //AuthenticateApiKeyPayload payload = new(value);
          //AuthenticateApiKeyCommand command = new(payload);
          //ApiKey apiKey = await _activityPipeline.ExecuteAsync(command, new ContextParameters());

          //Context.SetApiKey(apiKey);

          //ClaimsPrincipal principal = new(apiKey.CreateClaimsIdentity(Scheme.Name));
          //AuthenticationTicket ticket = new(principal, Scheme.Name);

          //return AuthenticateResult.Success(ticket);
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
