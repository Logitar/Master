using Logitar.Master.Application.Sessions;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.Web.Extensions;
using Logitar.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Logitar.Master.Authentication;

internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
{
  private readonly ISessionQuerier _sessionQuerier;

  public SessionAuthenticationHandler(ISessionQuerier sessionQuerier, IOptionsMonitor<SessionAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _sessionQuerier = sessionQuerier;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    string? sessionId = Context.GetSessionId();
    if (sessionId != null)
    {
      Session? session = await _sessionQuerier.ReadAsync(sessionId); // TODO(fpion): caching
      if (session == null)
      {
        return Fail($"The session 'Id={sessionId}' could not be found.");
      }
      else if (!session.IsActive)
      {
        return Fail($"The session 'Id={session.Id}' has ended.");
      }
      else if (session.User == null)
      {
        return Fail($"The session 'Id={session.Id}' has no user.");
      }
      else if (session.User.IsDisabled)
      {
        return Fail($"The User is disabled for session 'Id={session.Id}'.");
      }

      Context.SetSession(session);
      Context.SetUser(session.User);

      ClaimsPrincipal principal = new(CreateClaimsIdentity(session, Scheme.Name) /* session.CreateClaimsIdentity(Scheme.Name) */); // TODO(fpion): implement
      AuthenticationTicket ticket = new(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }

    return AuthenticateResult.NoResult();
  }

  private AuthenticateResult Fail(string reason)
  {
    Context.SignOut();

    return AuthenticateResult.Fail(reason);
  }

  private static ClaimsIdentity CreateClaimsIdentity(Session session, string? authenticationType = null)
  {
    // ClaimsIdentity identity = session.User.CreateClaimsIdentity(authenticationType); // TODO(fpion): implement
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id));

    return identity;
  }
}
