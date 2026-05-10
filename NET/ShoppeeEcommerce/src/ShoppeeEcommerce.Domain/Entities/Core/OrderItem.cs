using ShoppeeEcommerce.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class OrderItem
        : BaseEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        // No reference to product
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        // Store the main img
        public string? ProductImgUrl { get; set; }
        public decimal PriceSnapshot { get; set; }
        public int Quantity { get; set; }

        [NotMapped]
        public decimal SubTotal => PriceSnapshot * Quantity;

        public static OrderItem CreateItem(Product product, int quantity)
        {
            return new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSKU = product.SKU,
                ProductImgUrl = product.MainImage?.Url,
                PriceSnapshot = product.Price,
                Quantity = quantity
            };
        }
    }
}
