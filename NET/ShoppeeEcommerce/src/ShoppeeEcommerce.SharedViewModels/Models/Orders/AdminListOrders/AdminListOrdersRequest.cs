namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders
{
    public record AdminListOrdersRequest
    {
        public string? Status { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
        public DateTime? FromCreatedDate { get; init; }
        public DateTime? ToCreatedDate { get; init; }
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; } = false;
        public int? PageIndex { get; init; } = 1;
        public int? PageSize { get; init; } = 10;
    }
}
