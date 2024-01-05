using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IAccountService _accountService;

  public AccountController(IAccountService accountService)
  {
    _accountService = accountService;
  }

  [HttpPost("register")]
  public async Task<ActionResult<Session>> RegisterAsync([FromBody] RegisterPayload payload, CancellationToken cancellationToken) // TODO(fpion): no return?
  {
    Session session = await _accountService.RegisterAsync(payload, cancellationToken);
    // TODO(fpion): sign-in user

    return Ok(session); // TODO(fpion): NoContent();
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken) // TODO(fpion): no return?
  {
    Session session = await _accountService.SignInAsync(payload, cancellationToken);
    // TODO(fpion): sign-in user

    return Ok(session); // TODO(fpion): NoContent();
  }
}
