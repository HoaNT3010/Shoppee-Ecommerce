using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.WebAPI.Common.Validators.Query
{
    // Ok name
    public class PagedRequestValidator<T>
        : AbstractValidator<T>
        where T : PagedRequest
    {
        public PagedRequestValidator()
        {
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page index must be at least 1.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
        }
    }
}
