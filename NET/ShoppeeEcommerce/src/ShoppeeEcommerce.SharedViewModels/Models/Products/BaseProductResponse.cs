using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public record BaseProductResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string SKU { get; init; } = string.Empty;
        public List<BaseProductImageResponse> Images { get; init; } = new List<BaseProductImageResponse>();
        public List<ShortCategoryResponse> Categories { get; init; } = new List<ShortCategoryResponse>();
    }
}
