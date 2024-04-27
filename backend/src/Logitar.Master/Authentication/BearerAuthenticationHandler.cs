using Logitar.Master.Constants;
using Logitar.Master.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Master.Authentication;

internal class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IActivityPipeline _activityPipeline;

  public BearerAuthenticationHandler(IActivityPipeline activityPipeline, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _activityPipeline = activityPipeline;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            ValidateTokenPayload payload = new(values[1])
            {
              Type = "at+jwt"
            };
            ValidateTokenCommand command = new(payload);
            ValidatedToken validatedToken = await _activityPipeline.ExecuteAsync(command, new ContextParameters());
            if (string.IsNullOrWhiteSpace(validatedToken.Subject))
            {
              return AuthenticateResult.Fail($"The '{nameof(validatedToken.Subject)}' claim is required.");
            }

            ReadUserQuery query = new(Id: Guid.Parse(validatedToken.Subject.Trim()), UniqueName: null, Identifier: null);
            User? user = await _activityPipeline.ExecuteAsync(query, new ContextParameters());
            if (user == null)
            {
              return AuthenticateResult.Fail($"The user 'Id={validatedToken.Subject}' could not be found.");
            }

            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
