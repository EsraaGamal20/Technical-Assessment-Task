using App.Application.Features.Tasks.Dtos;

namespace App.Application.Features.Projects.Dtos;

public sealed record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<TaskDto> Tasks);
