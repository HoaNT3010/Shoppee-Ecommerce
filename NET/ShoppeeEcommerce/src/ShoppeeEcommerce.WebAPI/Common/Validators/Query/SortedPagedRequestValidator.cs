using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.WebAPI.Common.Validators.Query
{
    // Kinda bad name
    public class SortedPagedRequestValidator<T>
        : PagedRequestValidator<T>
        where T : SortedPagedRequest
    {
        public SortedPagedRequestValidator(IReadOnlyCollection<string> allowedSortFields)
        {
            RuleFor(x => x.SortBy)
                .MaximumLength(100)
                .Must(value => allowedSortFields.Contains(value, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"SortBy must be one of: {string.Join(", ", allowedSortFields)}.")
                .When(x => x.SortBy is not null);
        }
    }
}
