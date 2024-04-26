using Logitar.Master.Domain.Projects;
using Logitar.Portal.Contracts.Projects;

namespace Logitar.Master.Application.Projects;

public interface IProjectQuerier
{
  Task<Project> ReadAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(ProjectId id, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Project?> ReadAsync(string uniqueKey, CancellationToken cancellationToken = default);
}
