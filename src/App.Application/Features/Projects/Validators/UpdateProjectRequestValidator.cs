using App.Application.Common.Constants;
using App.Application.Features.Projects.Dtos;
using FluentValidation;

namespace App.Application.Features.Projects.Validators;

public sealed class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ValidationConstants.ProjectNameMin)
            .MaximumLength(ValidationConstants.ProjectNameMax);

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.ProjectDescMax)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
