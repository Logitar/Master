using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Master.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Repositories;

internal class ProjectRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IProjectRepository
{
  private static readonly string AggregateType = typeof(ProjectAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ProjectRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IEnumerable<ProjectAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProjectAggregate>(cancellationToken);
  }

  public async Task<ProjectAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProjectAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<ProjectAggregate?> LoadAsync(ProjectId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProjectAggregate>(id.AggregateId, cancellationToken);
  }
  public async Task<ProjectAggregate?> LoadAsync(ProjectId id, long? version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ProjectAggregate>(id.AggregateId, version, cancellationToken);
  }

  public async Task<ProjectAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(MasterDb.Projects.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(MasterDb.Projects.UniqueKeyNormalized, Operators.IsEqualTo(MasterDb.Normalize(uniqueKey.Value)))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProjectAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ProjectAggregate project, CancellationToken cancellationToken)
  {
    await base.SaveAsync(project, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<ProjectAggregate> projects, CancellationToken cancellationToken)
  {
    await base.SaveAsync(projects, cancellationToken);
  }
}
