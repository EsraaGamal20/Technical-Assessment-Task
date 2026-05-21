namespace App.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    private static void ApplyAudit(DbContext context)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is null) continue;

            var hasCreated   = entry.Metadata.FindProperty("CreatedAt") is not null;
            var hasUpdated   = entry.Metadata.FindProperty("UpdatedAt") is not null;
            var hasIsDeleted = entry.Metadata.FindProperty("IsDeleted") is not null;

            switch (entry.State)
            {
                case EntityState.Added when hasCreated:
                    entry.Property("CreatedAt").CurrentValue = utcNow;
                    break;

                case EntityState.Modified when hasUpdated:
                    entry.Property("UpdatedAt").CurrentValue = utcNow;
                    break;

                case EntityState.Deleted when hasIsDeleted:
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                    if (hasUpdated)
                        entry.Property("UpdatedAt").CurrentValue = utcNow;
                    break;
            }
        }
    }
}
