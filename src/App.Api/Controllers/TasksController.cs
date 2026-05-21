using App.Application.Common.Models;
using App.Application.Features.Tasks.Dtos;
using App.Application.Features.Tasks.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public sealed class TasksController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromRoute] Guid projectId,[FromBody]  CreateTaskRequest request,CancellationToken ct)
    {
        var result = await _taskService.CreateAsync(projectId, request, ct);
        return ToActionResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<TaskDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProject(
        [FromRoute] Guid projectId,
        [FromQuery] PaginationRequest request,
        CancellationToken ct)
    {
        var result = await _taskService.GetByProjectAsync(projectId, request, ct);
        return ToActionResult(result);
    }


    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        CancellationToken ct)
    {
        var result = await _taskService.GetByIdAsync(projectId, taskId, ct);
        return ToActionResult(result);
    }


    [HttpPatch("{taskId:guid}/status")]
    [ProducesResponseType(typeof(Result<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result),StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        [FromBody]  UpdateTaskStatusRequest request,
        CancellationToken ct)
    {
        var result = await _taskService.UpdateStatusAsync(projectId, taskId, request, ct);
        return ToActionResult(result);
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid projectId,
        [FromRoute] Guid taskId,
        CancellationToken ct)
    {
        var result = await _taskService.DeleteAsync(projectId, taskId, ct);
        return ToActionResult(result);
    }

}
