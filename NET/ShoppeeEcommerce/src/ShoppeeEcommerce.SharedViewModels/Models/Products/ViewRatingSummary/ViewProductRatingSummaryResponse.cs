namespace ShoppeeEcommerce.SharedViewModels.Models.Products.ViewRatingSummary
{
    public class ViewProductRatingSummaryResponse
    {
        public Guid ProductId { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalCount { get; set; }
        //public int TotalWithReviews { get; set; }
        //public int TotalVerifiedPurchases { get; set; }
        public ProductRatingDistributionResponse Distribution { get; set; } = new();
    }

    public class ProductRatingDistributionResponse
    {
        public ProductRatingDistributionItemResponse Star1 { get; set; } = new();
        public ProductRatingDistributionItemResponse Star2 { get; set; } = new();
        public ProductRatingDistributionItemResponse Star3 { get; set; } = new();
        public ProductRatingDistributionItemResponse Star4 { get; set; } = new();
        public ProductRatingDistributionItemResponse Star5 { get; set; } = new();
    }

    public class ProductRatingDistributionItemResponse
    {
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
