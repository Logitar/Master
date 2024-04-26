namespace Logitar.Master.Domain.Projects;

public interface IProjectRepository
{
  Task<IEnumerable<ProjectAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<ProjectAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ProjectAggregate?> LoadAsync(ProjectId id, CancellationToken cancellationToken = default);
  Task<ProjectAggregate?> LoadAsync(ProjectId id, long? version, CancellationToken cancellationToken = default);
  Task<ProjectAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ProjectAggregate> projects, CancellationToken cancellationToken = default);
}
