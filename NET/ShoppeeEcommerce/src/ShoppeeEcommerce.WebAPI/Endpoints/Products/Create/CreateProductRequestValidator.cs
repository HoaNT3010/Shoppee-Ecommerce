using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.Create;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.Create
{
    public class CreateProductRequestValidator
        : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Product's name is required.")
                .MaximumLength(200)
                .WithMessage("Product's name cannot exceed 200 characters.");
            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .WithMessage("Product's description is required.")
                .MaximumLength(1000)
                .WithMessage("Product's description cannot exceed 1000 characters.");
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Product's price must be greater than 0.")
                .PrecisionScale(18, 2, true)
                .WithMessage("Product's price must have at most 2 decimal places.");
            RuleFor(x => x.SKU)
                .NotNull()
                .NotEmpty()
                .WithMessage("Product's SKU is required")
                .MaximumLength(128)
                .WithMessage("SKU must not exceed 128 characters.")
                .Matches("^[a-zA-Z0-9-_]+$")
                .WithMessage("SKU must only contain letters, numbers, hyphens, and underscores.");
            RuleFor(x => x.CategoryIds)
                .NotEmpty()
                .WithMessage("At least one category is required.")
                .Must(ids => ids.Count <= 10)
                .WithMessage("A product cannot belong to more than 10 categories.")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate categories are not allowed.");
            RuleForEach(x => x.CategoryIds)
                .MustBeGuid()
                .WithMessage("Category ID must be a valid GUID/UUID.");
        }
    }
}
