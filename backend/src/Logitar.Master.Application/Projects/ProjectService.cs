using Logitar.Master.Application.Projects.Commands;
using Logitar.Master.Application.Projects.Queries;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using MediatR;

namespace Logitar.Master.Application.Projects;

internal class ProjectService : IProjectService
{
  private readonly IMediator _mediator;

  public ProjectService(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Project?> CreateAsync(CreateProjectPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateProjectCommand(payload), cancellationToken);
  }

  public async Task<Project?> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteProjectCommand(id), cancellationToken);
  }

  public async Task<Project?> ReadAsync(string? id, string? uniqueKey, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadProjectQuery(id, uniqueKey), cancellationToken);
  }

  public async Task<Project?> ReplaceAsync(string id, ReplaceProjectPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceProjectCommand(id, payload, version), cancellationToken);
  }

  public async Task<SearchResults<Project>> SearchAsync(SearchProjectsPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SearchProjectsQuery(payload), cancellationToken);
  }

  public async Task<Project?> UpdateAsync(string id, UpdateProjectPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateProjectCommand(id, payload), cancellationToken);
  }
}
