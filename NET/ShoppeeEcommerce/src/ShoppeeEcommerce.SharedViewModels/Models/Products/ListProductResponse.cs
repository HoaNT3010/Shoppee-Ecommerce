namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public record ListProductResponse : BaseProductSummaryResponse
    {
        public bool IsFeatured { get; init; }
    }
}
