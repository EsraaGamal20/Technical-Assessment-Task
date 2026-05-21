namespace App.Application.Features.Projects.Dtos;

public sealed record UpdateProjectRequest(
    string  Name,
    string? Description);
