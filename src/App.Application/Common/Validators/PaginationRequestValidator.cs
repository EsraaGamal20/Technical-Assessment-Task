using App.Application.Common.Models;
using FluentValidation;

namespace App.Application.Common.Validators;

public sealed class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));
        RuleFor(x => x.SortBy)
            .MaximumLength(60)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }
}
