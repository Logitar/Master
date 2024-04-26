using Logitar.Master.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore;

public class MasterContext : DbContext
{
  public MasterContext(DbContextOptions<MasterContext> options) : base(options)
  {
  }

  internal DbSet<ActorEntity> Actors { get; private set; }
  internal DbSet<ProjectEntity> Projects { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
