using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational;

/// <summary>
/// The database context of the Master system.
/// </summary>
public class MasterContext : DbContext
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MasterContext"/> class.
  /// </summary>
  /// <param name="options"></param>
  public MasterContext(DbContextOptions<MasterContext> options) : base(options)
  {
  }

  internal DbSet<ActorEntity> Actors { get; private set; }
  internal DbSet<ProjectEntity> Projects { get; private set; }

  /// <summary>
  /// Configures the model builder.
  /// </summary>
  /// <param name="modelBuilder">The model builder.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
