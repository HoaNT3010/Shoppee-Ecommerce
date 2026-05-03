using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts;
using ShoppeeEcommerce.WebAPI.Common.Validators.Query;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.AdminListProducts
{
    public class AdminListProductsRequestValidator
        : DateRangesSortedPagedIncludeDeletedRequestValidator<AdminListProductsRequest>
    {
        private static readonly string[] _allowedSorts = [
            "name",
            "createdDate",
            "isDeleted",
            "status",
            "price",
            "sku"
        ];

        private static readonly string[] _statuses = ["Draft", "Published"];

        public AdminListProductsRequestValidator() : base(_allowedSorts)
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term must not exceed 100 characters.")
                .When(x => x.SearchTerm is not null);
            RuleFor(x => x.Status)
                .Must(x => _statuses.Contains(x))
                .WithMessage("Product status filter must be one of the following: draft, published.")
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
        }
    }
}
