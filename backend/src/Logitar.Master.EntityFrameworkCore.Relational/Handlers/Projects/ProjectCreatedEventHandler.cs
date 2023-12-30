using Logitar.Master.Domain.Projects.Events;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Handlers.Projects;

internal class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
{
  private readonly MasterContext _context;

  public ProjectCreatedEventHandler(MasterContext context)
  {
    _context = context;
  }

  public async Task Handle(ProjectCreatedEvent @event, CancellationToken cancellationToken)
  {
    ProjectEntity? project = await _context.Projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (project == null)
    {
      project = new(@event);
      _context.Projects.Add(project);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
