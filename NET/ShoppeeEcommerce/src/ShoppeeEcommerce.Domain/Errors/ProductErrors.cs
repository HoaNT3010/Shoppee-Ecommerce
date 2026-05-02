using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class ProductErrors
        {
            public static Error ProductNameExisted(string name) =>
                Error.Conflict(
                    "Product.ProductNameExisted",
                    $"The name '{name}' has been used by another product.");
            public static Error ProductSKUExisted(string sku) =>
                Error.Conflict(
                    "Product.ProductSKUExisted",
                    $"The SKU '{sku}' has been used by another product.");
            public static Error CreateProductFailed() =>
                Error.Failure(
                    "Product.CreateProductFailed",
                    "Unexpected error occurred when trying to create new product.");
            public static Error ProductNotFound() =>
                Error.NotFound(
                    "Product.ProductNotFound",
                    "Product was not found.");
            public static Error ProductNotFoundWithId(string id) =>
                Error.NotFound(
                    "Product.ProductNotFoundWithId",
                    $"Product with ID '{id}' was not found.");

            public static Error UpdateFailed() =>
                Error.Failure(
                    "Product.UpdateFailed",
                    "Unexpected error occurred when trying to update product.");
            public static Error SoftDeleteFailed() =>
                Error.Failure(
                    "Product.SoftDeleteFailed",
                    "Unexpected error occurred when trying to soft delete product.");
            public static Error RestoreSoftDeletedFailed() =>
                Error.Failure(
                    "Product.RestoreSoftDeletedFailed",
                    "Unexpected error occurred when trying to restore soft deleted product.");
            public static Error HardDeleteFailed() =>
                Error.Failure(
                    "Product.HardDeleteFailed",
                    "Unexpected error occurred when trying to hard delete product.");

            public static Error ExceedMaximumCategories(int limit) =>
                Error.Validation(
                    "Product.ExceedMaximumCategories",
                    $"Product has exceed maximum number of categories ({limit} categories).");
            public static Error ExceedMaximumImages(int limit) =>
                Error.Validation(
                    "Product.ExceedMaximumImages",
                    $"Product has exceed maximum number of images ({limit} images).");
            public static Error UpdateImagesFailed(string id) =>
                Error.Failure(
                    "Product.UpdateImagesFailed",
                    $"Unexpected error occurred when trying to update images for product with ID '{id}'.");
            public static Error UpdateStatusFailed() =>
                Error.Failure(
                    "Product.UpdateStatusFailed",
                    "Unexpected error occurred when trying to update product's status.");
            public static Error EmptyProductImages() =>
                Error.NotFound(
                    "Product.EmptyProductImages",
                    "Product does not have any images.");
            public static Error HasNoSpecificImage(int imageId, string productId) =>
                Error.NotFound(
                    "Product.HasNoSpecificImage",
                    $"Image with ID '{imageId}' not found on product '{productId}'.");
            public static Error UpdateMainImageFailed(string id) =>
                Error.Failure(
                    "Product.UpdateMainImageFailed",
                    $"Unexpected error occurred when trying to update main image for product with ID '{id}'.");

            public static Error PublishProductFailed(string id) =>
                Error.Failure(
                    "Product.PublishProductFailed",
                    $"Unexpected error occurred when trying to publish product with ID '{id}'.");
            public static Error AlreadyPublished(string id) =>
                Error.Conflict(
                    "Product.AlreadyPublished",
                    $"Product with ID '{id}' has already been published.");
            public static Error MissingImages =>
                Error.Validation(
                    "Product.MissingImages",
                    "Product must have at least one image before publishing.");
            public static Error MissingMainImage =>
                Error.Validation(
                    "Product.MissingMainImage",
                    "Product must have a main image before publishing.");
            public static Error MissingCategories =>
                Error.Validation(
                    "Product.MissingCategories",
                    "Product must belong to at least one category before publishing.");
            public static Error InvalidPrice =>
                Error.Validation(
                    "Product.InvalidPrice",
                    "Product must have a valid price before publishing.");
            public static Error HasNoImages =>
                Error.Validation(
                    "Product.HasNoImages",
                    "Product does not have any images.");
            public static Error ImagesNotFound(int[] imageIds) =>
                Error.Validation(
                    "Product.ImagesNotFound",
                    $"Product does not have any image(s) with ID: {string.Join(", ", imageIds)}");
            public static Error ImagesMissingFromReorder(int[] imageIds) =>
                Error.Validation(
                    "Product.ImagesMissingFromReorder",
                    $"Please provide all product's images with display order. The image(s) with ID is missing: {string.Join(", ", imageIds)}");
        }
    }
}
