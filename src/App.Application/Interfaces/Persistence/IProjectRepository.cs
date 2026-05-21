using App.Application.Common.Models;
using App.Domain.Entities;

namespace App.Application.Interfaces.Persistence;

public interface IProjectRepository : IGenericRepository<Project, Guid>
{
    Task<Project?> GetByIdForUserAsync(Guid projectId, Guid userId, CancellationToken ct = default);
    Task<Project?> GetWithTasksAsync(Guid projectId, Guid userId, CancellationToken ct = default);

    Task<(IReadOnlyList<Project> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId,
        PaginationRequest request,
        CancellationToken ct = default);
}
