using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Master.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Repositories;

internal class ProjectRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IProjectRepository
{
  private static readonly string _aggregateType = typeof(ProjectAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ProjectRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ProjectAggregate?> LoadAsync(ProjectId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<ProjectAggregate?> LoadAsync(ProjectId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<ProjectAggregate>(id.AggregateId, version, cancellationToken);

  public async Task<ProjectAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken)
  {
    string uniqueKeyNormalized = uniqueKey.Value.ToUpper();

    IQuery query = _sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Projects.AggregateId, Db.Events.AggregateId, new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(_aggregateType)))
      .Where(Db.Projects.UniqueKeyNormalized, Operators.IsEqualTo(uniqueKeyNormalized))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ProjectAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ProjectAggregate project, CancellationToken cancellationToken)
    => await base.SaveAsync(project, cancellationToken);
}
