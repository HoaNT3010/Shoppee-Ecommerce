using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.Create;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.Create
{
    public class CreateCategoryRequestValidator
        : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage("Category's name cannot be null, empty or contain only white space(s).")
                .MaximumLength(100)
                .WithMessage("Category's name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .Must(d => !string.IsNullOrWhiteSpace(d))
                .WithMessage("Category's description cannot be null, empty or contain only white space(s).")
                .MaximumLength(1000)
                .WithMessage("Category's description cannot exceed 1000 characters.");
        }
    }
}
