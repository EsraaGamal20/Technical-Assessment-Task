using App.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(Result<T> result) => StatusCode(result.StatusCode, result);
    protected IActionResult ToActionResult(Result result)       => StatusCode(result.StatusCode, result);
}
