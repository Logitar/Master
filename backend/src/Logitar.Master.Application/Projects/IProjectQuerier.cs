using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects;

public interface IProjectQuerier
{
  Task<Project> ReadAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(ProjectId id, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(string uniqueKey, CancellationToken cancellationToken = default);
}
