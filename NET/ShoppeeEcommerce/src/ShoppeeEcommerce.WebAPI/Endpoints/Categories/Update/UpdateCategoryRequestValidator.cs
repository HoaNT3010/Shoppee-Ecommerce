using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.Update;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.Update
{
    public class UpdateCategoryRequestValidator
        : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category's ID is required.");
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
