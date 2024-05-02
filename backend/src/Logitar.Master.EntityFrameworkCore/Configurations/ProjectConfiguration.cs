using Logitar.Identity.Domain.Shared;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Master.EntityFrameworkCore.Configurations;

internal class ProjectConfiguration : AggregateConfiguration<ProjectEntity>, IEntityTypeConfiguration<ProjectEntity>
{
  public override void Configure(EntityTypeBuilder<ProjectEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(MasterContext.Projects));
    builder.HasKey(x => x.ProjectId);

    builder.HasIndex(x => x.UniqueKey);
    builder.HasIndex(x => x.UniqueKeyNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueKey).HasMaxLength(UniqueKeyUnit.MaximumLength);
    builder.Property(x => x.UniqueKeyNormalized).HasMaxLength(UniqueKeyUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
  }
}
