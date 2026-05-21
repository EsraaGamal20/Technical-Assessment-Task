using App.Application.Common.Models;
using App.Application.Features.Projects.Dtos;

namespace App.Application.Features.Projects.Services;

public interface IProjectService
{
    Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request, CancellationToken ct = default);
    Task<Result<PagedResult<ProjectDto>>> GetAllAsync(PaginationRequest request, CancellationToken ct = default);
    Task<Result<ProjectDetailsDto>>  GetByIdAsync(Guid projectId, CancellationToken ct = default);
    Task<Result<ProjectDto>> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid projectId, CancellationToken ct = default);
}
