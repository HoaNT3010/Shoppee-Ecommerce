using FluentValidation;

namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders
{
    public class AdminListOrdersRequestValidator
        : AbstractValidator<AdminListOrdersRequest>
    {
        static readonly string[] _statuses = ["Pending", "AwaitingPayment", "Paid", "Completed", "Cancelled", "Refunded"];
        private static readonly string[] _allowedSorts = [
            "createdDate",
            "totalPrice",
        ];
        public AdminListOrdersRequestValidator()
        {
            RuleFor(x => x.Status)
                .Must(x => _statuses.Contains(x))
                .WithMessage($"Order status filter must be one of: {string.Join(", ", _statuses)}.")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));
            // Only Start price provided
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Min price must be greater or equal to 0.");
            // Only End price provided
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Max price must be greater or equal to 0.");
            // Both are provided
            RuleFor(x => x)
                .Must(x =>
                    !x.MinPrice.HasValue ||
                    !x.MaxPrice.HasValue ||
                    x.MinPrice.Value <= x.MaxPrice.Value)
                .WithMessage("Min price must be less than or equal to Max price.");
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
            // Only From Date provided
            RuleFor(x => x.FromCreatedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.FromCreatedDate.HasValue)
                .WithMessage("Start date cannot be in the future.");
            // Only To Date provided
            RuleFor(x => x.ToCreatedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.ToCreatedDate.HasValue)
                .WithMessage("End date cannot be in the future.");
            // Both are provided
            RuleFor(x => x)
                .Must(x =>
                    !x.FromCreatedDate.HasValue ||
                    !x.ToCreatedDate.HasValue ||
                    x.FromCreatedDate.Value <= x.ToCreatedDate.Value)
                .WithMessage("Start date must be earlier than or equal to End date.");
        }
    }
}
