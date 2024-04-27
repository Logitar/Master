using Logitar.Master.Application.Accounts.Commands;
using Logitar.Master.Authentication;
using Logitar.Master.Contracts.Accounts;
using Logitar.Master.Extensions;
using Logitar.Master.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly IBearerTokenService _bearerTokenService;
  private readonly ISender _sender;

  public AccountController(IBearerTokenService bearerTokenService, ISender sender)
  {
    _bearerTokenService = bearerTokenService;
    _sender = sender;
  }

  [HttpPost("/auth/sign/in")]
  public async Task<ActionResult<SignInResponse>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    SignInCommandResult result = await _sender.Send(new SignInCommand(payload), cancellationToken);
    if (result.Session != null)
    {
      HttpContext.SignIn(result.Session);
    }

    return Ok(new SignInResponse(result));
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<GetTokenResponse>> TokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(payload.RefreshToken))
    {
      throw new NotImplementedException(); // TODO(fpion): renew session
    }

    SignInCommandResult result = await _sender.Send(new SignInCommand(payload), cancellationToken);
    GetTokenResponse response = new(result);
    if (result.Session != null)
    {
      response.TokenResponse = _bearerTokenService.GetTokenResponse(result.Session);
    }

    return Ok(response);
  }
}
