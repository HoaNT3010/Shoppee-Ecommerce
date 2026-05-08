using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class CartItem
        : BaseEntity<int>
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = default!;

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; }

        public void Increase(int quantity) => Quantity += quantity;
        public void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException("Item quantity must be greater than 0.");
            Quantity = quantity;
        }
    }
}
