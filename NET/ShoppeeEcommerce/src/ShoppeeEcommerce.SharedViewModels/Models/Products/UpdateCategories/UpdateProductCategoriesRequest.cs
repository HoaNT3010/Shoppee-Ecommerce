using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateCategories
{
    public class UpdateProductCategoriesRequest
    {
        [FromRoute(Name = "id")]
        public string? Id { get; set; }

        [FromBody]
        public ProductCategoriesRequest? Categories { get; set; }
    }

    public class ProductCategoriesRequest
    {
        public List<string> CategoryIds { get; set; } = [];
    }
}
