namespace ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts
{
    public class ListProductsRequest
    {
        public string? SearchTerm { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
        public List<string>? CategoryIds { get; set; } = null;
        public bool? IsFeatured { get; set; } = null;
        // Sorting
        public string? SortBy { get; set; } = null;
        public bool? SortDesc { get; set; } = null;
        // Paging
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
