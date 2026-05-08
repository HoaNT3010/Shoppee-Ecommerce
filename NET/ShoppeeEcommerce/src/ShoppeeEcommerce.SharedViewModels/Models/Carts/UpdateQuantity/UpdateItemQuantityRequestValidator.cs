using FluentValidation;

namespace ShoppeeEcommerce.SharedViewModels.Models.Carts.UpdateQuantity
{
    public class UpdateItemQuantityRequestValidator
        : AbstractValidator<UpdateItemQuantityRequest>
    {
        public UpdateItemQuantityRequestValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Quantity)
                .NotNull()
                .WithMessage("Item quantity must be provided.");
            RuleFor(x => x.Quantity.Quantity)
                .GreaterThan(0)
                .WithMessage("Item quantity must be greater than 0.");
        }
    }
}
