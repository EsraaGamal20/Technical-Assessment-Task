using App.Application.Common.Constants;
using App.Application.Features.Tasks.Dtos;
using FluentValidation;

namespace App.Application.Features.Tasks.Validators;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MinimumLength(ValidationConstants.TaskTitleMin)
            .MaximumLength(ValidationConstants.TaskTitleMax);

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.TaskDescMax)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority value is not supported.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
