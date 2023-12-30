using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Master.Application.Projects;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Relational.Actors;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Queriers;

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
    return await ReadAsync(project.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The project 'Id={project.Id.Value}' could not be found.");
  }

  public async Task<Project?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    ProjectEntity? project = await _projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  public async Task<Project?> ReadByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = uniqueKey.Trim().ToUpper();

    ProjectEntity? project = await _projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueKeyNormalized == uniqueKeyNormalized, cancellationToken);
    if (project == null)
    {
      return null;
    }

    return project == null ? null : await MapAsync(project, cancellationToken);
  }

  public async Task<SearchResults<Project>> SearchAsync(SearchProjectsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Projects.Table).SelectAll(Db.Projects.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Projects.AggregateId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Projects.UniqueKey, Db.Projects.DisplayName);

    IQueryable<ProjectEntity> query = _projects.FromQuery(builder).AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ProjectEntity>? ordered = null;
    foreach (ProjectSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ProjectSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case ProjectSort.UniqueKey:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueKey) : query.OrderBy(x => x.UniqueKey))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueKey) : ordered.ThenBy(x => x.UniqueKey));
          break;
        case ProjectSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ProjectEntity[] projects = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Project> results = await MapAsync(projects, cancellationToken);

    return new SearchResults<Project>(results, total);
  }

  private async Task<Project> MapAsync(ProjectEntity project, CancellationToken cancellationToken)
    => (await MapAsync(new[] { project }, cancellationToken)).Single();
  private async Task<IEnumerable<Project>> MapAsync(IEnumerable<ProjectEntity> projects, CancellationToken cancellationToken)
  {
    ActorId[] actorIds = projects.SelectMany(project => project.GetActorIds()).Distinct().ToArray();
    IEnumerable<Actor> actors = await _actorService.ResolveAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return projects.Select(mapper.ToProject);
  }
}
