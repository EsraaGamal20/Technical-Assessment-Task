using App.Application.Common.Models;
using App.Domain.Entities;

namespace App.Application.Interfaces.Persistence;

public interface ITaskRepository : IGenericRepository<TaskItem, Guid>
{
    Task<TaskItem?> GetByIdForUserAsync(Guid taskId, Guid userId, CancellationToken ct = default);

    Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> GetByProjectAsync(
        Guid projectId,
        Guid userId,
        PaginationRequest request,
        CancellationToken ct = default);
}
