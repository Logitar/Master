using Logitar.Master.Application;
using Logitar.Master.Application.Accounts;
using Logitar.Master.Application.Accounts.Commands;
using Logitar.Master.Authentication;
using Logitar.Master.Contracts.Accounts;
using Logitar.Master.Extensions;
using Logitar.Master.Models.Account;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly IBearerTokenService _bearerTokenService;
  private readonly IRequestPipeline _requestPipeline;
  private readonly ISessionService _sessionService;

  public AccountController(IBearerTokenService bearerTokenService, IRequestPipeline requestPipeline, ISessionService sessionService)
  {
    _bearerTokenService = bearerTokenService;
    _requestPipeline = requestPipeline;
    _sessionService = sessionService;
  }

  // TODO(fpion): get profile

  [HttpPost("/auth/sign/in")]
  public async Task<ActionResult<SignInResponse>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    SignInCommandResult result = await _requestPipeline.ExecuteAsync(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    if (result.Session != null)
    {
      HttpContext.SignIn(result.Session);
    }

    return Ok(new SignInResponse(result));
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<GetTokenResponse>> TokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    GetTokenResponse response;
    Session? session;
    if (!string.IsNullOrWhiteSpace(payload.RefreshToken))
    {
      response = new GetTokenResponse();
      session = await _sessionService.RenewAsync(payload.RefreshToken, HttpContext.GetSessionCustomAttributes(), cancellationToken);
    }
    else
    {
      SignInCommandResult result = await _requestPipeline.ExecuteAsync(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
      response = new(result);
      session = result.Session;
    }

    if (session != null)
    {
      response.TokenResponse = _bearerTokenService.GetTokenResponse(session);
    }

    return Ok(response);
  }
}
