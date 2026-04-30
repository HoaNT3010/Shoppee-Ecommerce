using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.UploadImages
{
    public class UploadProductImagesRequest
    {
        [FromRoute(Name = "id")]
        public string? Id { get; set; }
        [FromForm]
        public IFormFileCollection Images { get; set; } = null!;
    }
}
