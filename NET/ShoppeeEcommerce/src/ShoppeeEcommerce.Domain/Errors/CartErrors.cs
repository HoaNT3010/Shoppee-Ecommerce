using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class CartErrors
        {
            public static Error AddItemFailed() =>
                Error.Failure(
                    "Cart.AddItemFailed",
                    "Unexpected error occurred when trying to add item to cart.");
            public static Error ClearItemsFailed() =>
                Error.Failure(
                    "Cart.ClearItemsFailed",
                    "Unexpected error occurred when trying to clear items in cart.");
            public static Error RemoveItemFailed() =>
                Error.Failure(
                    "Cart.RemoveItemFailed",
                    "Unexpected error occurred when trying to remove item in cart.");
            public static Error UpdateItemQuantityFailed() =>
                Error.Failure(
                    "Cart.UpdateItemQuantityFailed",
                    "Unexpected error occurred when trying to update an item's quantity in cart.");
            public static Error GetCartFailed() =>
                Error.Failure(
                    "Cart.GetCartFailed",
                    "Unexpected error occurred when trying to get cart information.");
            public static Error CreateCartFailed() =>
                Error.Failure(
                    "Cart.CreateCartFailed",
                    "Unexpected error occurred when trying to create cart for user.");
        }
    }
}
