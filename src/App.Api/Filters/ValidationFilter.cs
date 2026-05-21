using App.Application.Common.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Api.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new List<Error>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var argumentType  = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                errors.AddRange(result.Errors.Select(e =>
                    new Error(e.PropertyName, e.ErrorMessage)));
            }
        }

        if (errors.Count > 0)
        {
            var failure = Result.ValidationFailure(errors);
            context.Result = new ObjectResult(failure) { StatusCode = failure.StatusCode };
            return;
        }

        await next();
    }
}
