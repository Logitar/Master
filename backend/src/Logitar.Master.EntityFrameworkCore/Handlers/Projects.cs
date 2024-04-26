using Logitar.Master.Domain.Projects.Events;
using Logitar.Master.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Handlers;

internal static class Projects
{
  public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
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

  public class ProjectDeletedEventHandler : INotificationHandler<ProjectDeletedEvent>
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

  public class ProjectUpdatedEventHandler : INotificationHandler<ProjectUpdatedEvent>
  {
    private readonly MasterContext _context;

    public ProjectUpdatedEventHandler(MasterContext context)
    {
      _context = context;
    }

    public async Task Handle(ProjectUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ProjectEntity project = await _context.Projects
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The project entity 'AggregateId={@event.AggregateId}' could not be found.");

      project.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
