namespace App.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.OwnerId).IsRequired();

        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasMany(x => x.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OwnerId)
            .HasDatabaseName("IX_Projects_OwnerId");

        builder.HasIndex(x => new { x.OwnerId, x.Name })
            .HasDatabaseName("IX_Projects_Owner_Name");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
