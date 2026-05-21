namespace App.Application.Features.Projects.Dtos;

public sealed record CreateProjectRequest(
    string  Name,
    string? Description);
