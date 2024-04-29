using Logitar.Master.Application.Accounts;
using Logitar.Master.Constants;
using Logitar.Master.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Logitar.Master.Authentication;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IUserService _userService;

  public BasicAuthenticationHandler(IUserService userService, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _userService = userService;
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
        else if (values[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(values[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }

          try
          {
            User user = await _userService.AuthenticateAsync(uniqueName: credentials[..index], password: credentials[(index + 1)..]);

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
