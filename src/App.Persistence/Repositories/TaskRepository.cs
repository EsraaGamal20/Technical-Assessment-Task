namespace App.Persistence.Repositories;

public sealed class TaskRepository
    : GenericRepository<TaskItem, Guid>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public Task<TaskItem?> GetByIdForUserAsync(
        Guid taskId, Guid userId, CancellationToken ct = default)
        => Set.Include(t => t.Project)
              .FirstOrDefaultAsync(t => t.Id == taskId && t.Project!.OwnerId == userId, ct);

    public async Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetByProjectAsync(
        Guid projectId, Guid userId, PaginationRequest request, CancellationToken ct = default)
    {
        var query = Set.AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.Project!.OwnerId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(t =>
                t.Title.Contains(term) ||
                (t.Description != null && t.Description.Contains(term)));
        }

        query = (request.SortBy?.ToLowerInvariant(), request.SortDescending) switch
        {
            ("title", true)      => query.OrderByDescending(t => t.Title),
            ("title", false)     => query.OrderBy(t => t.Title),
            ("priority", true)   => query.OrderByDescending(t => t.Priority),
            ("priority", false)  => query.OrderBy(t => t.Priority),
            ("duedate", true)    => query.OrderByDescending(t => t.DueDate),
            ("duedate", false)   => query.OrderBy(t => t.DueDate),
            ("status", true)     => query.OrderByDescending(t => t.Status),
            ("status", false)    => query.OrderBy(t => t.Status),
            _                    => query.OrderByDescending(t => t.CreatedAt)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
