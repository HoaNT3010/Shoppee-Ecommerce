using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts
{
    public record AdminListProductsRequest
        : DateRangesSortedPagedIncludeDeletedRequest
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
    }
}
