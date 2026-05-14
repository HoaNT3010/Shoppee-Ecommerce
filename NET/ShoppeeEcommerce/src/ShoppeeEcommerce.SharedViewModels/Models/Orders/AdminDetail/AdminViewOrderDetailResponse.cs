namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminDetail
{
    public class AdminViewOrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<AdminOrderItemDetailResponse> Items { get; set; } = [];
        public AdminOrderPaymentResponse? Payment { get; set; }
    }

    public class AdminOrderItemDetailResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public string? ProductImgUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class AdminOrderPaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? RefundedDate { get; set; }
        public DateTime? FailedDate { get; set; }
        public string? FailureReason { get; set; }
        public decimal? RefundedAmount { get; set; }
    }
}
