using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateInfo;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UpdateInfo
{
    public class UpdateProductInfoRequestValidator
        : AbstractValidator<UpdateProductInfoRequest>
    {
        public UpdateProductInfoRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();
            RuleFor(x => x.Info!.Name)
                .MaximumLength(200)
                .WithMessage("Product's name cannot exceed 200 characters.")
                .When(x => x.Info != null && !string.IsNullOrWhiteSpace(x.Info.Name));
            RuleFor(x => x.Info!.Description)
                .MaximumLength(1000)
                .WithMessage("Product's description cannot exceed 1000 characters.")
                .When(x => x.Info != null && !string.IsNullOrWhiteSpace(x.Info.Description));
            RuleFor(x => x.Info!.Price)
                .GreaterThan(0)
                .WithMessage("Product's price must be greater than 0.")
                .PrecisionScale(18, 2, true)
                .WithMessage("Product's price must have at most 2 decimal places.")
                .When(x => x.Info != null && x.Info.Price != null && x.Info.Price.HasValue);
            RuleFor(x => x.Info!.SKU)
                .MaximumLength(128)
                .WithMessage("SKU must not exceed 128 characters.")
                .Matches("^[a-zA-Z0-9-_]+$")
                .WithMessage("SKU must only contain letters, numbers, hyphens, and underscores.")
                .When(x => x.Info != null && !string.IsNullOrWhiteSpace(x.Info.SKU));
        }
    }
}
