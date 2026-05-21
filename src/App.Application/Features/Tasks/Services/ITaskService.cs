using App.Application.Common.Models;
using App.Application.Features.Tasks.Dtos;

namespace App.Application.Features.Tasks.Services;

public interface ITaskService
{
    Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct = default);
    Task<Result<PagedResult<TaskDto>>> GetByProjectAsync(Guid projectId, PaginationRequest request, CancellationToken ct = default);
    Task<Result<TaskDto>> GetByIdAsync(Guid projectId, Guid taskId, CancellationToken ct = default);
    Task<Result<TaskDto>>UpdateStatusAsync(Guid projectId, Guid taskId, UpdateTaskStatusRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid projectId, Guid taskId, CancellationToken ct = default);
}
