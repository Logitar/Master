using Logitar.Master.Constants;
using Logitar.Master.Contracts.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
[Authorize(Policies.SystemUser)]
[Route("sessions")]
public class SessionController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  /* TODO(fpion): implement
   * - CreateAsync
   * - SearchAsync
   */

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    Session? session = await _sessionService.ReadAsync(id, cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.RenewAsync(payload, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}");

    return Created(uri, session);
  }

  [HttpPut("sign/out/{id}")]
  public async Task<ActionResult<Session>> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignOutAsync(id, cancellationToken));
  }
}
