namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public class BaseProductSummaryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? ImgUrl { get; set; }
    }
}
