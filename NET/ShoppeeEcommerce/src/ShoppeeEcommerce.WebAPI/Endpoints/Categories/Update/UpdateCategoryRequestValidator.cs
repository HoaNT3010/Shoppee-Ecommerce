using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.Update;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.Update
{
    public class UpdateCategoryRequestValidator
        : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .MustBeGuid();
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Category's name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Category's description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
