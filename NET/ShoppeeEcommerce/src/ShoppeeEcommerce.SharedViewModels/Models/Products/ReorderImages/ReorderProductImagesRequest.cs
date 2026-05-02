using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.ReorderImages
{
    public class ReorderProductImagesRequest
    {
        [FromRoute(Name = "id")]
        public string? Id { get; set; }

        [FromBody]
        public ProductImageRequest Orders { get; set; } = new();
    }

    public class ProductImageRequest
    {
        public List<ProductImageItem> Orders { get; set; } = [];
    }

    public class ProductImageItem
    {
        public int ImageId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
