namespace App.Application.Features.Projects.Dtos;

public sealed record ProjectDto(
    Guid  Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TasksCount);
