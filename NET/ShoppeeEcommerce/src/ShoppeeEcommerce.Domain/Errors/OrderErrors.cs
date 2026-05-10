using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class OrderErrors
        {
            public static Error NotFoundWithId(string id) =>
                Error.NotFound(
                    "Order.NotFoundWithId",
                    $"Order with ID '{id}' was not found.");

            public static Error NoCartInfo() =>
                Error.NotFound(
                    "Order.NoCartInfo",
                    "No shopping cart associated with user found.");

            public static Error EmptyCart() =>
                Error.Validation(
                    "Order.EmptyCart",
                    "Order cannot be created with empty shopping cart.");

            public static Error GuestCartNotAllowed() =>
                Error.Validation(
                    "Order.GuestCartNotAllowed",
                    "Order cannot be created with guest shopping cart.");

            public static Error ItemCountNotMatch() =>
                Error.Validation(
                    "Order.ItemCountNotMatch",
                    "Order's products count does not match the number of products fetched from database.");

            public static Error ContainsInvalidProducts() =>
                Error.Validation(
                    "Order.ContainsInvalidProducts",
                    "Order cannot contain invalid product(s).");

            public static Error CreateOrderFailed() =>
                Error.Failure(
                    "Order.CreateOrderFailed",
                    "Unexpected error occurred when trying to create new order.");
        }
    }
}
