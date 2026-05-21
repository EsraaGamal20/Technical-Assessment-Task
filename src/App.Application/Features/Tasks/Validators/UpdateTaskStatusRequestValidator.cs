using App.Application.Features.Tasks.Dtos;
using FluentValidation;

namespace App.Application.Features.Tasks.Validators;

public sealed class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status value is not supported.");
    }
}
