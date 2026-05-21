using App.Application.Features.Projects.Dtos;
using App.Application.Features.Tasks.Dtos;
using App.Domain.Entities;

namespace App.Application.Mappings;

public static class ProjectMappings
{
    public static ProjectDto ToDto(this Project project) => new(
        project.Id,
        project.Name,
        project.Description,
        project.CreatedAt,
        project.UpdatedAt,
        project.Tasks?.Count(t => !t.IsDeleted) ?? 0);

    public static ProjectDetailsDto ToDetailsDto(this Project project) => new(
        project.Id,
        project.Name,
        project.Description,
        project.CreatedAt,
        project.UpdatedAt,
        project.Tasks?
            .Where(t => !t.IsDeleted)
            .Select(t => t.ToDto())
            .ToList() ?? new List<TaskDto>());

    public static Project ToEntity(this CreateProjectRequest request, Guid ownerId) => new()
    {
        Id  = Guid.NewGuid(),
        Name = request.Name.Trim(),
        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim(),
        OwnerId = ownerId,
        CreatedAt= DateTime.UtcNow
    };

    public static void ApplyUpdate(this Project project, UpdateProjectRequest request)
    {
        project.Name = request.Name.Trim();
        project.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim();
        project.UpdatedAt= DateTime.UtcNow;
    }
}
