using App.Application.Common.Constants;
using App.Application.Features.Auth.Dtos;
using FluentValidation;

namespace App.Application.Features.Auth.Validators;

public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ValidationConstants.PhonePattern)
            .WithMessage("Phone number must be in E.164 format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required.")
            .Length(ValidationConstants.OtpLength)
            .WithMessage($"OTP must be {ValidationConstants.OtpLength} digits.")
            .Matches(ValidationConstants.OtpPattern)
            .WithMessage("OTP must contain digits only.");
    }
}
