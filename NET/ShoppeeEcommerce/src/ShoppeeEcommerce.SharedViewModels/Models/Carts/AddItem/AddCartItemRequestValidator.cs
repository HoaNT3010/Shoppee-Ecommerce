using FluentValidation;

namespace ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem
{
    public class AddCartItemRequestValidator
        : AbstractValidator<AddCartItemRequest>
    {
        public AddCartItemRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Item quantity must be greater than 0.");
        }
    }
}
