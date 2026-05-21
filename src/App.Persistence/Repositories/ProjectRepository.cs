namespace App.Persistence.Repositories;

public sealed class ProjectRepository
    : GenericRepository<Project, Guid>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public Task<Project?> GetByIdForUserAsync(
        Guid projectId, Guid userId, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId, ct);

    public Task<Project?> GetWithTasksAsync(
        Guid projectId, Guid userId, CancellationToken ct = default)
        => Set.Include(p => p.Tasks.Where(t => !t.IsDeleted))
              .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId, ct);

    public async Task<(IReadOnlyList<Project> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId, PaginationRequest request, CancellationToken ct = default)
    {
        var query = Set.AsNoTracking().Where(p => p.OwnerId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                (p.Description != null && p.Description.Contains(term)));
        }

        query = (request.SortBy?.ToLowerInvariant(), request.SortDescending) switch
        {
            ("name", true)       => query.OrderByDescending(p => p.Name),
            ("name", false)      => query.OrderBy(p => p.Name),
            ("createdat", true)  => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            _                    => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .ToListAsync(ct);

        return (items, total);
    }
}
