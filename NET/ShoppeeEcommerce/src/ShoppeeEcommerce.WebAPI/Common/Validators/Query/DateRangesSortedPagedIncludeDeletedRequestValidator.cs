using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.WebAPI.Common.Validators.Query
{
    // Really bad name. The last chained name (for now, hopefully).
    public class DateRangesSortedPagedIncludeDeletedRequestValidator<T>
        : SortedPagedIncludeDeletedRequestValidator<T>
        where T : DateRangesSortedPagedIncludeDeletedRequest
    {
        public DateRangesSortedPagedIncludeDeletedRequestValidator(IReadOnlyCollection<string> allowedSortFields)
            : base(allowedSortFields)
        {
            // Only From Date provided
            RuleFor(x => x.FromCreatedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.FromCreatedDate.HasValue)
                .WithMessage("FromCreatedDate cannot be in the future.");
            // Only To Date provided
            RuleFor(x => x.ToCreatedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.ToCreatedDate.HasValue)
                .WithMessage("ToCreatedDate cannot be in the future.");
            // Both are provided
            RuleFor(x => x)
                .Must(x =>
                    !x.FromCreatedDate.HasValue ||
                    !x.ToCreatedDate.HasValue ||
                    x.FromCreatedDate.Value <= x.ToCreatedDate.Value)
                .WithMessage("FromCreatedDate must be earlier than or equal to ToCreatedDate.");
        }
    }
}
