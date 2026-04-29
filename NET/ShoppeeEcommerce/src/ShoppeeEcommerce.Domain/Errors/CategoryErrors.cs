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
            public static Error UpdateCategoryFailed() =>
                Error.Failure(
                    "Category.UpdateCategoryFailed",
                    "Unexpected error occurred when trying to update category.");
            public static Error NotFoundWithId(string id) =>
                Error.NotFound(
                    "Category.NotFoundWithId",
                    $"Category with ID '{id}' was not found.");
            public static Error SoftDeleteCategoryFailed() =>
                Error.Failure(
                    "Category.SoftDeleteCategoryFailed",
                    "Unexpected error occurred when trying to soft delete category.");
            public static Error RestoreSoftDeletedCategoryFailed() =>
                Error.Failure(
                    "Category.RestoreSoftDeletedCategoryFailed",
                    "Unexpected error occurred when trying to restore soft deleted category.");
            public static Error HardDeleteCategoryFailed() =>
                Error.Failure(
                    "Category.HardDeleteCategoryFailed",
                    "Unexpected error occurred when trying to hard delete category.");
            public static Error InvalidCategoryIds()
                => Error.Validation(
                    "Category.InvalidCategoryIds",
                    "One or more provided Category IDs are invalid.");
        }
    }
}
