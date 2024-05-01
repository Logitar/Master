using Logitar.Master.Application;
using Logitar.Master.Application.Accounts;
using Logitar.Master.Application.Accounts.Commands;
using Logitar.Master.Authentication;
using Logitar.Master.Contracts.Accounts;
using Logitar.Master.Extensions;
using Logitar.Master.Models.Account;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
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

  [HttpPut("/phone/change")]
  [Authorize]
  public async Task<ActionResult<ChangePhoneResult>> ChangePhoneAsync([FromBody] ChangePhonePayload payload, CancellationToken cancellationToken)
  {
    ChangePhoneResult result = await _requestPipeline.ExecuteAsync(new ChangePhoneCommand(payload), cancellationToken);
    return Ok(result);
  }

  [HttpPost("/phone/verify")]
  public async Task<ActionResult<VerifyPhoneResult>> VerifyPhoneAsync([FromBody] VerifyPhonePayload payload, CancellationToken cancellationToken)
  {
    VerifyPhoneResult result = await _requestPipeline.ExecuteAsync(new VerifyPhoneCommand(payload), cancellationToken);
    return Ok(result);
  }

  [HttpGet("/profile")]
  [Authorize]
  public ActionResult<UserProfile> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    return Ok(user.ToUserProfile());
  }
}
