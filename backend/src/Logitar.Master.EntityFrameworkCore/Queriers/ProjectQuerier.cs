using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Master.Application.Projects;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Actors;
using Logitar.Master.EntityFrameworkCore.Entities;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Queriers;

internal class ProjectQuerier : IProjectQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ProjectEntity> _projects;
  private readonly ISqlHelper _sqlHelper;

  public ProjectQuerier(IActorService actorService, MasterContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _projects = context.Projects;
    _sqlHelper = sqlHelper;
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

    ProjectEntity? project = await _projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  public async Task<Project?> ReadAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = MasterDb.Normalize(uniqueKey);

    ProjectEntity? project = await _projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  public async Task<SearchResults<Project>> SearchAsync(SearchProjectsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(MasterDb.Projects.Table).SelectAll(MasterDb.Projects.Table)
      .ApplyIdFilter(MasterDb.Projects.AggregateId, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, MasterDb.Projects.UniqueKey, MasterDb.Projects.DisplayName);

    IQueryable<ProjectEntity> query = _projects.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ProjectEntity>? ordered = null;
    if (payload.Sort != null && payload.Sort.Count > 0)
    {
      foreach (ProjectSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
          case ProjectSort.DisplayName:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenByDescending(x => x.DisplayName));
            break;
          case ProjectSort.UniqueKey:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueKey) : query.OrderBy(x => x.UniqueKey))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueKey) : ordered.ThenByDescending(x => x.UniqueKey));
            break;
          case ProjectSort.UpdatedOn:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenByDescending(x => x.UpdatedOn));
            break;
        }
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ProjectEntity[] projects = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Project> items = await MapAsync(projects, cancellationToken);

    return new SearchResults<Project>(items, total);
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
