namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<OrderItemDetailResponse> Items { get; set; } = [];
    }

    public class OrderItemDetailResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public string? ProductImgUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
