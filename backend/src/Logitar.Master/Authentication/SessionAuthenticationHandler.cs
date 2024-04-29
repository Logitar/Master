﻿using Logitar.Master.Application.Accounts;
using Logitar.Master.Extensions;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Logitar.Master.Authentication;

internal class SessionAuthenticationHandler : AuthenticationHandler<SessionAuthenticationOptions>
{
  private readonly ISessionService _sessionService;

  public SessionAuthenticationHandler(ISessionService sessionService, IOptionsMonitor<SessionAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _sessionService = sessionService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    Guid? sessionId = Context.GetSessionId();
    if (sessionId.HasValue)
    {
      Session? session = await _sessionService.FindAsync(sessionId.Value);
      if (session == null)
      {
        return Fail($"The session 'Id={sessionId}' could not be found.");
      }
      else if (!session.IsActive)
      {
        return Fail($"The session 'Id={session.Id}' has ended.");
      }
      else if (session.User.IsDisabled)
      {
        return Fail($"The User is disabled for session 'Id={session.Id}'.");
      }

      Context.SetSession(session);
      Context.SetUser(session.User);

      ClaimsPrincipal principal = new(session.CreateClaimsIdentity(Scheme.Name));
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
}
