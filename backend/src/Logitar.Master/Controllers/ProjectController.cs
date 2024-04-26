﻿using Logitar.Master.Application.Projects.Commands;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
  private readonly ISender _sender;

  public ProjectController(ISender sender)
  {
    _sender = sender; // TODO(fpion): ActivityPipeline or something like that
  }

  [HttpPost]
  public async Task<ActionResult<Project>> CreateAsync([FromBody] CreateProjectPayload payload, CancellationToken cancellationToken)
  {
    Project project = await _sender.Send(new CreateProjectCommand(payload), cancellationToken);
    return Created(BuildLocation(project), project);
  }

  private Uri BuildLocation(Project project) => HttpContext.BuildLocation("projects/{id}", new Dictionary<string, string> { ["id"] = project.Id.ToString() });
}
