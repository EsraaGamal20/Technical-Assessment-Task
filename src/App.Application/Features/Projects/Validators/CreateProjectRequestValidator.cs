using App.Application.Common.Constants;
using App.Application.Features.Projects.Dtos;
using FluentValidation;

namespace App.Application.Features.Projects.Validators;

public sealed class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MinimumLength(ValidationConstants.ProjectNameMin)
            .MaximumLength(ValidationConstants.ProjectNameMax);

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.ProjectDescMax)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
