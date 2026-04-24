using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories;
using ShoppeeEcommerce.WebAPI.Common.Validators.Query;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.AdminListCategories
{
    public class AdminListCategoriesRequestValidator
        : DateRangesSortedPagedIncludeDeletedRequestValidator<AdminListCategoriesRequest>
    {
        private static readonly string[] _allowedSorts = [
            "name",
            "createdDate",
            "isDeleted"];

        public AdminListCategoriesRequestValidator() : base(_allowedSorts)
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term must not exceed 100 characters.")
                .When(x => x.SearchTerm is not null);
        }
    }
}
