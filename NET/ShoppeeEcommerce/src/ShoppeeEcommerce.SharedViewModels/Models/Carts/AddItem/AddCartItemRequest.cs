namespace ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem
{
    public class AddCartItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
