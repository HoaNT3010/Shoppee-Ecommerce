using FluentValidation;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.GetFeatured
{
    public class GetFeaturedProductsRequestValidator
        : AbstractValidator<GetFeaturedProductsRequest>
    {
        public GetFeaturedProductsRequestValidator()
        {
            RuleFor(x => x.Count)
                .GreaterThan(0)
                .WithMessage("The total number of products must be greater than 0.")
                .LessThanOrEqualTo(50)
                .WithMessage("The total number of products must be less than or equal to 50")
                .When(x => x.Count.HasValue);
        }
    }
}
