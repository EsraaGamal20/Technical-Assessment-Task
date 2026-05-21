using App.Application.Common.Constants;
using App.Application.Common.Exceptions;
using App.Application.Common.Models;
using App.Application.Features.Tasks.Dtos;
using App.Application.Interfaces.Persistence;
using App.Application.Interfaces.Services;
using App.Application.Mappings;
using App.Domain.Entities;
using App.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace App.Application.Features.Tasks.Services;

public sealed class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService  _currentUser;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        IUnitOfWork uow,
        ICurrentUserService currentUser,
        IDateTimeProvider clock,
        ILogger<TaskService> logger)
    {
        _uow= uow;
        _currentUser = currentUser;
        _clock = clock;
        _logger= logger;
    }

    public async Task<Result<TaskDto>> CreateAsync(
        Guid projectId, CreateTaskRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();
        await EnsureProjectOwnedAsync(projectId, ownerId, ct);

        var task = request.ToEntity(projectId);
        await _uow.Tasks.AddAsync(task, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Task {TaskId} created in project {ProjectId} by user {UserId}",
            task.Id, projectId, ownerId);

        return Result<TaskDto>.Success(task.ToDto(), statusCode: 201);
    }

    public async Task<Result<PagedResult<TaskDto>>> GetByProjectAsync(
        Guid projectId, PaginationRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();
        await EnsureProjectOwnedAsync(projectId, ownerId, ct);

        var (items, total) = await _uow.Tasks.GetByProjectAsync(projectId, ownerId, request, ct);

        var dtos  = items.Select(t => t.ToDto()).ToList();
        var paged = PagedResult<TaskDto>.Create(dtos, request.PageNumber, request.PageSize, total);

        return Result<PagedResult<TaskDto>>.Success(paged);
    }

    public async Task<Result<TaskDto>> GetByIdAsync(
        Guid projectId, Guid taskId, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();
        await EnsureProjectOwnedAsync(projectId, ownerId, ct);

        var task = await _uow.Tasks.GetByIdForUserAsync(taskId, ownerId, ct)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.ProjectId != projectId)
            throw new NotFoundException(nameof(TaskItem), taskId);

        return Result<TaskDto>.Success(task.ToDto());
    }

    public async Task<Result<TaskDto>> UpdateStatusAsync(
        Guid projectId, Guid taskId, UpdateTaskStatusRequest request, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();
        await EnsureProjectOwnedAsync(projectId, ownerId, ct);

        var task = await _uow.Tasks.GetByIdForUserAsync(taskId, ownerId, ct)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.ProjectId != projectId)
            throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.Status == request.Status)
            return Result<TaskDto>.Success(task.ToDto());

        if (!IsValidTransition(task.Status, request.Status))
            throw new BusinessRuleException(
                $"Cannot change status from {task.Status} to {request.Status}.",
                ErrorCodes.BusinessRuleViolation);

        task.Status = request.Status;
        task.UpdatedAt = _clock.UtcNow;
        _uow.Tasks.Update(task);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Task {TaskId} status -> {Status} by user {UserId}", taskId, request.Status, ownerId);

        return Result<TaskDto>.Success(task.ToDto());
    }

    public async Task<Result> DeleteAsync(
        Guid projectId, Guid taskId, CancellationToken ct = default)
    {
        var ownerId = _currentUser.GetUserIdOrThrow();
        await EnsureProjectOwnedAsync(projectId, ownerId, ct);

        var task = await _uow.Tasks.GetByIdForUserAsync(taskId, ownerId, ct)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.ProjectId != projectId)
            throw new NotFoundException(nameof(TaskItem), taskId);

        _uow.Tasks.Remove(task);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Task {TaskId} deleted by user {UserId}", taskId, ownerId);

        return Result.Success("Task deleted.");
    }

    private async Task EnsureProjectOwnedAsync(Guid projectId, Guid ownerId, CancellationToken ct)
    {
        var exists = await _uow.Projects.ExistsAsync(
            p => p.Id == projectId && p.OwnerId == ownerId, ct);

        if (!exists)
            throw new NotFoundException(nameof(Project), projectId);
    }

    private static bool IsValidTransition(TaskItemStatus from, TaskItemStatus to) => from switch
    {
        TaskItemStatus.Todo       => to is TaskItemStatus.InProgress or TaskItemStatus.Cancelled,
        TaskItemStatus.InProgress => to is TaskItemStatus.Done or TaskItemStatus.Todo or TaskItemStatus.Cancelled,
        TaskItemStatus.Done       => to is TaskItemStatus.InProgress,
        TaskItemStatus.Cancelled  => to is TaskItemStatus.Todo,
        _                         => false
    };
}
