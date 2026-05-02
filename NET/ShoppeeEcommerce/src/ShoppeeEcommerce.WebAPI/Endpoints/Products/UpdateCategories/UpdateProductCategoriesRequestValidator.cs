using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateCategories;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UpdateCategories
{
    public class UpdateProductCategoriesRequestValidator
        : AbstractValidator<UpdateProductCategoriesRequest>
    {
        public UpdateProductCategoriesRequestValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id)
                .MustBeGuid();
            RuleFor(x => x.Categories)
                .NotNull()
                .WithMessage("Product's category(s) is required.");
            RuleFor(x => x.Categories!.CategoryIds)
                .NotEmpty()
                .WithMessage("At least one category is required.")
                .Must(ids => ids.Count <= 10)
                .WithMessage("A product cannot belong to more than 10 categories.")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate categories are not allowed.")
                .When(x => x.Categories is not null);
            RuleForEach(x => x.Categories!.CategoryIds)
                .MustBeGuid()
                .WithMessage("Category ID must be a valid GUID/UUID.");
        }
    }
}
