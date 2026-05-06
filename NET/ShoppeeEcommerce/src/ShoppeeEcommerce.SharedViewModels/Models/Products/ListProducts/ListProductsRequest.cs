namespace ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts
{
    public class ListProductsRequest
    {
        public string? SearchTerm { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
        public List<string>? CategoryIds { get; set; } = null;
        public bool? IsFeatured { get; set; } = false;
        // Sorting
        public string? SortBy { get; set; } = null;
        public bool? SortDesc { get; set; } = false;
        // Paging
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
