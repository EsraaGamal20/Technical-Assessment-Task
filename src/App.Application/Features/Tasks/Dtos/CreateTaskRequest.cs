using App.Domain.Enums;

namespace App.Application.Features.Tasks.Dtos;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate);
