using App.Domain.Enums;

namespace App.Application.Features.Tasks.Dtos;

public sealed record TaskDto(
    Guid  Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    string  StatusName,
    TaskPriority  Priority,
    string PriorityName,
    DateTime? DueDate,
    Guid ProjectId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
