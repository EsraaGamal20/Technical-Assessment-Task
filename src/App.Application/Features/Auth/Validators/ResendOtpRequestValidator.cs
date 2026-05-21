using App.Application.Common.Constants;
using App.Application.Features.Auth.Dtos;
using FluentValidation;

namespace App.Application.Features.Auth.Validators;

public sealed class ResendOtpRequestValidator : AbstractValidator<ResendOtpRequest>
{
    public ResendOtpRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ValidationConstants.PhonePattern)
            .WithMessage("Phone number must be in E.164 format.");
    }
}
