namespace ShoppeeEcommerce.SharedViewModels.Models.Carts.View
{
    public class ViewCartResponse
    {
        public List<ViewCartItemResponse> Items { get; set; } = [];
        public bool HasInactiveItem { get; set; }
        public bool HasDeletedItem { get; set; }
        public bool HasPriceChangedItem { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }

    public class ViewCartItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImgUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal? OldPrice { get; set; }
        public CartItemStatus Status { get; set; }

    }

    public enum CartItemStatus
    {
        Available,
        Inactive,
        PriceChanged
    }
}
