using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateInfo
{
    public class UpdateProductInfoRequest
    {
        [FromRoute(Name = "id")]
        public string? Id { get; set; }

        [FromBody]
        public ProductInfoRequest? Info { get; set; }
    }

    public class ProductInfoRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? SKU { get; set; }
    }
}
