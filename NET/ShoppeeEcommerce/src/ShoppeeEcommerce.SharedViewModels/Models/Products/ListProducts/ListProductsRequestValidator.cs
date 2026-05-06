using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Validators;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts
{
    public class ListProductsRequestValidator
        : AbstractValidator<ListProductsRequest>
    {
        private static readonly string[] _allowedSorts = [
            "name",
            "createdDate",
            "price",
        ];
        public ListProductsRequestValidator()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term must not exceed 100 characters.")
                .When(x => x.SearchTerm is not null);
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
            RuleFor(x => x.CategoryIds)
                .Must(ids => ids!.Count <= 10)
                .WithMessage("Cannot filter by more than 10 categories.")
                .Must(ids => ids!.Distinct().Count() == ids!.Count)
                .WithMessage("Duplicate category IDs are not allowed.")
                .ForEach(x => x.MustBeGuid())
                .When(x => x.CategoryIds?.Count > 0);
            // Sorting
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
