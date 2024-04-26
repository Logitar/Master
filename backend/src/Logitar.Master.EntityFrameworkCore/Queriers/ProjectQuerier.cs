using Logitar.EventSourcing;
using Logitar.Master.Application.Projects;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Actors;
using Logitar.Master.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Queriers;

internal class ProjectQuerier : IProjectQuerier
{
  private readonly IActorService _actorService;
  private readonly MasterContext _context;

  public ProjectQuerier(IActorService actorService, MasterContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task<Project> ReadAsync(ProjectAggregate project, CancellationToken cancellationToken)
  {
    return await ReadAsync(project.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The project entity 'AggregateId={project.Id.AggregateId}' could not be found.");
  }
  public async Task<Project?> ReadAsync(ProjectId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Project?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ProjectEntity? project = await _context.Projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  public async Task<Project?> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = MasterDb.Normalize(uniqueKey);

    ProjectEntity? project = await _context.Projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  private async Task<Project> MapAsync(ProjectEntity project, CancellationToken cancellationToken)
  {
    return (await MapAsync([project], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<Project>> MapAsync(IEnumerable<ProjectEntity> projects, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = projects.SelectMany(project => project.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return projects.Select(mapper.ToProject).ToArray();
  }
}
