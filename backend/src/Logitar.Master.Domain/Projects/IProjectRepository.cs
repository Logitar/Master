namespace Logitar.Master.Domain.Projects;

public interface IProjectRepository
{
  Task<ProjectAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken = default);

  Task SaveAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ProjectAggregate> projects, CancellationToken cancellationToken = default);
}
