using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ReorderImages;
using ShoppeeEcommerce.WebAPI.Common.Validators;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.ReorderImages
{
    public class ReorderProductImagesRequestValidator
        : AbstractValidator<ReorderProductImagesRequest>
    {
        public ReorderProductImagesRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();
            RuleFor(x => x.Orders)
                .NotEmpty()
                .WithMessage("At least one image order is required.");

            // Display orders must be unique — no two images with the same position
            RuleFor(x => x.Orders)
                .Must(orders => orders.Orders.Select(o => o.DisplayOrder).Distinct().Count() == orders.Orders.Count)
                .WithMessage("Duplicate display orders are not allowed.");

            RuleForEach(x => x.Orders.Orders)
                .ChildRules(order =>
                {
                    order.RuleFor(x => x.ImageId)
                        .GreaterThan(0).WithMessage("ImageId must be valid.");

                    order.RuleFor(x => x.DisplayOrder)
                        .GreaterThan(0).WithMessage("DisplayOrder must be greater than 0.");
                });
        }
    }
}
