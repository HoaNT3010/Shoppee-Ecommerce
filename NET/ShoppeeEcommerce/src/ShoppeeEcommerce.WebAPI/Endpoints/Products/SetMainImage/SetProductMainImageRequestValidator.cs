using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.SetMainImage;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.SetMainImage
{
    public class SetProductMainImageRequestValidator
        : AbstractValidator<SetProductMainImageRequest>
    {
        public SetProductMainImageRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();
            RuleFor(x => x.ImageId)
                .GreaterThan(0)
                .WithMessage("Image ID must greater than 0.");
        }
    }
}
