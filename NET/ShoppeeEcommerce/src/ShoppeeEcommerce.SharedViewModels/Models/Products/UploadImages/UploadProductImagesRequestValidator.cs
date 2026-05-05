using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Validators;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.UploadImages
{
    public class UploadProductImagesRequestValidator
        : AbstractValidator<UploadProductImagesRequest>
    {
        public UploadProductImagesRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();

            RuleFor(x => x.Images)
                .MustBeValidImageCollection(1, 10);
            RuleForEach(x => x.Images)
                .MustBeValidImage();
        }
    }
}
