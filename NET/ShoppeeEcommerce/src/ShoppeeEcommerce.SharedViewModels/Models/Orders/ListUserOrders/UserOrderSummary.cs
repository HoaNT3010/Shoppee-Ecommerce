namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders
{
    public class UserOrderSummary
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int LineItemsCount { get; set; }
    }
}
