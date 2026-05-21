using App.Domain.Enums;

namespace App.Application.Features.Tasks.Dtos;

public sealed record UpdateTaskStatusRequest(TaskItemStatus Status);
