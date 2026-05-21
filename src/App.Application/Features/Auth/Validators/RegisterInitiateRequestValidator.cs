using App.Application.Common.Constants;
using App.Application.Features.Auth.Dtos;
using FluentValidation;

namespace App.Application.Features.Auth.Validators;

public sealed class RegisterInitiateRequestValidator : AbstractValidator<RegisterInitiateRequest>
{
    public RegisterInitiateRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(ValidationConstants.FullNameMin)
            .MaximumLength(ValidationConstants.FullNameMax);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(ValidationConstants.EmailMax);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(ValidationConstants.PhonePattern)
            .WithMessage("Phone number must be in E.164 format (e.g. +201234567890).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(ValidationConstants.PasswordMin)
            .MaximumLength(ValidationConstants.PasswordMax)
            .Matches(ValidationConstants.PasswordPattern)
            .WithMessage("Password must contain upper, lower, digit, and a special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}
