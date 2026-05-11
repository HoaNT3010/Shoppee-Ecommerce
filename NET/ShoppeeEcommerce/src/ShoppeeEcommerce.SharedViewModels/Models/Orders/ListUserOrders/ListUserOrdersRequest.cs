namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders
{
    public class ListUserOrdersRequest
    {
        public string? Status { get; set; } = null;
        // Sorting
        public string? SortBy { get; set; } = null;
        public bool? SortDesc { get; set; } = null;
        // Paging
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
