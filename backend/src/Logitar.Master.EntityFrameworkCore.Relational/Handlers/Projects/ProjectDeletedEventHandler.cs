using Logitar.Master.Domain.Projects.Events;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Handlers.Projects;

internal class ProjectDeletedEventHandler : INotificationHandler<ProjectDeletedEvent>
{
  private readonly MasterContext _context;

  public ProjectDeletedEventHandler(MasterContext context)
  {
    _context = context;
  }

  public async Task Handle(ProjectDeletedEvent @event, CancellationToken cancellationToken)
  {
    ProjectEntity? project = await _context.Projects
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (project != null)
    {
      _context.Projects.Remove(project);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
