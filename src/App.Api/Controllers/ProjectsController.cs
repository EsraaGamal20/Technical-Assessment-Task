using App.Application.Common.Models;
using App.Application.Features.Projects.Dtos;
using App.Application.Features.Projects.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[Route("api/projects")]
[Authorize]
public sealed class ProjectsController : ApiControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
        => _projectService = projectService;

    [HttpPost]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var result = await _projectService.CreateAsync(request, ct);
        return ToActionResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<ProjectDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),                          StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest request, CancellationToken ct)
    {
        var result = await _projectService.GetAllAsync(request, ct);
        return ToActionResult(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),                    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),                    StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _projectService.GetByIdAsync(id, ct);
        return ToActionResult(result);
    }


    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result),             StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody]  UpdateProjectRequest request,CancellationToken ct)
    {
        var result = await _projectService.UpdateAsync(id, request, ct);
        return ToActionResult(result);
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _projectService.DeleteAsync(id, ct);
        return ToActionResult(result);
    }
}
