namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public record BaseProductSummaryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string SKU { get; init; } = string.Empty;
        public string? ImgUrl { get; init; }
    }
}
