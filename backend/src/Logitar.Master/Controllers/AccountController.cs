using Logitar.Master.Constants;
using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.Contracts.Users;
using Logitar.Master.Models.Account;
using Logitar.Master.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IAccountService _accountService;
  private readonly ISessionService _sessionService;

  public AccountController(IAccountService accountService, ISessionService sessionService)
  {
    _accountService = accountService;
    _sessionService = sessionService;
  }

  /* TODO(fpion): implement
   * - RecoverPasswordAsync
   * - ResetPasswordAsync
   * - SaveProfileAsync
   */

  [Authorize(Policies.SystemUser)]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("The user context item has not been set.");
    return Ok(user);
  }

  [HttpPost("register")]
  public async Task<ActionResult<CurrentUser>> RegisterAsync([FromBody] RegisterPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _accountService.RegisterAsync(payload, cancellationToken);
    HttpContext.SignIn(session);

    return Ok(GetCurrentUser(session));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    payload.TenantId = null;
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/sessions/{session.Id}");

    HttpContext.SignIn(session);

    return Created(uri, GetCurrentUser(session));
  }

  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    string? id = HttpContext.GetSessionId();
    if (id != null)
    {
      _ = await _sessionService.SignOutAsync(id, cancellationToken);
    }

    HttpContext.SignOut();

    return NoContent();
  }

  private static CurrentUser GetCurrentUser(Session session)
  {
    if (session.User == null)
    {
      throw new ArgumentException($"The {nameof(session.User)} is required.", nameof(session));
    }

    return new CurrentUser(session.User);
  }
}
