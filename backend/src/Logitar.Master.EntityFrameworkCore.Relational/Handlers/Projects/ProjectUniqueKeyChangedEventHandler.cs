using Logitar.Master.Domain.Projects.Events;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Handlers.Projects;

internal class ProjectUniqueKeyChangedEventHandler : INotificationHandler<ProjectUniqueKeyChangedEvent>
{
  private readonly MasterContext _context;

  public ProjectUniqueKeyChangedEventHandler(MasterContext context)
  {
    _context = context;
  }

  public async Task Handle(ProjectUniqueKeyChangedEvent @event, CancellationToken cancellationToken)
  {
    ProjectEntity? project = await _context.Projects
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (project != null)
    {
      project.SetUniqueKey(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
