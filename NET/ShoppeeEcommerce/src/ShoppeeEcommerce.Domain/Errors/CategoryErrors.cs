using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class CategoryErrors
        {
            public static Error CategoryNameDuplicated(string categoryName) =>
                Error.Conflict(
                    "Category.CategoryNameDuplicated",
                    $"The category name '{categoryName}' has been used by another category.");
            public static Error CreateCategoryFailed() =>
                Error.Failure(
                    "Category.CreateCategoryFailed",
                    "Unexpected error occurred when trying to create new category.");
        }
    }
}
