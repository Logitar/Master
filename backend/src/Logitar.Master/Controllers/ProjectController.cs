using Logitar.Master.Application.Projects.Commands;
using Logitar.Master.Application.Projects.Queries;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using Logitar.Master.Extensions;
using Logitar.Master.Models.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Master.Controllers;

[ApiController]
[Authorize]
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

  [HttpDelete("{id}")]
  public async Task<ActionResult<Project>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Project? project = await _sender.Send(new DeleteProjectCommand(id), cancellationToken);
    return project == null ? NotFound() : Ok(project);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Project>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Project? project = await _sender.Send(new ReadProjectQuery(id, UniqueKey: null), cancellationToken);
    return project == null ? NotFound() : Ok(project);
  }

  [HttpGet("key:{uniqueKey}")]
  public async Task<ActionResult<Project>> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    Project? project = await _sender.Send(new ReadProjectQuery(Id: null, uniqueKey), cancellationToken);
    return project == null ? NotFound() : Ok(project);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Project>> ReplaceAsync(Guid id, [FromBody] ReplaceProjectPayload payload, long? version, CancellationToken cancellationToken)
  {
    Project? project = await _sender.Send(new ReplaceProjectCommand(id, payload, version), cancellationToken);
    return project == null ? NotFound() : Ok(project);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Project>>> SearchAsync([FromQuery] SearchProjectsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<Project> projects = await _sender.Send(new SearchProjectsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(projects);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<Project>> UpdateAsync(Guid id, [FromBody] UpdateProjectPayload payload, CancellationToken cancellationToken)
  {
    Project? project = await _sender.Send(new UpdateProjectCommand(id, payload), cancellationToken);
    return project == null ? NotFound() : Ok(project);
  }

  private Uri BuildLocation(Project project) => HttpContext.BuildLocation("projects/{id}", new Dictionary<string, string> { ["id"] = project.Id.ToString() });
}
