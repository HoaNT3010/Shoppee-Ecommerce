using FluentValidation;

namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders
{
    public class ListUserOrdersRequestValidator
        : AbstractValidator<ListUserOrdersRequest>
    {
        static readonly string[] _statuses = ["Pending", "AwaitingPayment", "Paid", "Completed", "Cancelled", "Refunded"];
        private static readonly string[] _allowedSorts = [
            "createdDate",
            "totalPrice",
        ];

        public ListUserOrdersRequestValidator()
        {
            RuleFor(x => x.Status)
                .Must(x => _statuses.Contains(x))
                .WithMessage($"Order status filter must be one of: {string.Join(", ", _statuses)}.")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));
            RuleFor(x => x.SortBy)
                .MaximumLength(100)
                .Must(value => _allowedSorts.Contains(value, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"SortBy must be one of: {string.Join(", ", _allowedSorts)}.")
                .When(x => x.SortBy is not null);
            // Paging
            RuleFor(x => x.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page index must be at least 1.")
                .When(x => x.PageIndex.HasValue);
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.")
                .When(x => x.PageSize.HasValue);
        }
    }
}
