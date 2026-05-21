using App.Application.Common.Constants;
using App.Application.Common.Exceptions;
using App.Application.Common.Models;
using App.Application.Features.Projects.Dtos;
using App.Application.Interfaces.Persistence;
using App.Application.Interfaces.Services;
using App.Application.Mappings;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace App.Application.Features.Projects.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        ILogger<ProjectService> logger)
    {
        _uow = uow;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<ProjectDto>> CreateAsync(
        CreateProjectRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();

        var nameTaken = await _uow.Projects.ExistsAsync(
            p => p.OwnerId == ownerId && p.Name.ToLower() == request.Name.Trim().ToLower(),
            ct);

        if (nameTaken)
            throw new ConflictException(
                "You already have a project with this name.",
                ErrorCodes.Conflict);

        var project = request.ToEntity(ownerId);
        await _uow.Projects.AddAsync(project, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Project {ProjectId} created for user {UserId}", project.Id, ownerId);

        return Result<ProjectDto>.Success(project.ToDto(), statusCode: 201);
    }

    public async Task<Result<PagedResult<ProjectDto>>> GetAllAsync(
        PaginationRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();

        var (items, total) = await _uow.Projects.GetPagedForUserAsync(ownerId, request, ct);

        var dtos  = items.Select(p => p.ToDto()).ToList();
        var paged = PagedResult<ProjectDto>.Create(dtos, request.PageNumber, request.PageSize, total);

        return Result<PagedResult<ProjectDto>>.Success(paged);
    }

    public async Task<Result<ProjectDetailsDto>> GetByIdAsync(
        Guid projectId, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();

        var project = await _uow.Projects.GetWithTasksAsync(projectId, ownerId, ct)
            ?? throw new NotFoundException(nameof(Project), projectId);

        return Result<ProjectDetailsDto>.Success(project.ToDetailsDto());
    }

    public async Task<Result<ProjectDto>> UpdateAsync(
        Guid projectId, UpdateProjectRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();

        var project = await _uow.Projects.GetByIdForUserAsync(projectId, ownerId, ct)
            ?? throw new NotFoundException(nameof(Project), projectId);

        var newName   = request.Name.Trim();
        var duplicate = await _uow.Projects.ExistsAsync(
            p => p.OwnerId == ownerId
              && p.Id      != projectId
              && p.Name.ToLower() == newName.ToLower(),
            ct);

        if (duplicate)
            throw new ConflictException(
                "You already have a project with this name.",
                ErrorCodes.Conflict);

        project.ApplyUpdate(request);
        _uow.Projects.Update(project);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Project {ProjectId} updated by user {UserId}", project.Id, ownerId);

        return Result<ProjectDto>.Success(project.ToDto());
    }

    public async Task<Result> DeleteAsync(
        Guid projectId, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();

        var project = await _uow.Projects.GetByIdForUserAsync(projectId, ownerId, ct)
            ?? throw new NotFoundException(nameof(Project), projectId);

        _uow.Projects.Remove(project);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Project {ProjectId} deleted by user {UserId}", projectId, ownerId);

        return Result.Success("Project deleted.");
    }
}
