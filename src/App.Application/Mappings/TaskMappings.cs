using App.Application.Features.Tasks.Dtos;
using App.Domain.Entities;
using App.Domain.Enums;

namespace App.Application.Mappings;

public static class TaskMappings
{
    public static TaskDto ToDto(this TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.Status.ToString(),
        task.Priority,
        task.Priority.ToString(),
        task.DueDate,
        task.ProjectId,
        task.CreatedAt,
        task.UpdatedAt);

    public static TaskItem ToEntity(this CreateTaskRequest request, Guid projectId) => new()
    {
        Id          = Guid.NewGuid(),
        Title       = request.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description!.Trim(),
        Status      = TaskItemStatus.Todo,
        Priority    = request.Priority,
        DueDate     = request.DueDate,
        ProjectId   = projectId,
        CreatedAt   = DateTime.UtcNow
    };
}
