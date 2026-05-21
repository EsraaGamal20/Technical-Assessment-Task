namespace App.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Priority).HasConversion<int>().IsRequired();

        builder.Property(x => x.DueDate);
        builder.Property(x => x.ProjectId).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.ProjectId)
            .HasDatabaseName("IX_Tasks_ProjectId");

        builder.HasIndex(x => new { x.ProjectId, x.Status })
            .HasDatabaseName("IX_Tasks_Project_Status");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
