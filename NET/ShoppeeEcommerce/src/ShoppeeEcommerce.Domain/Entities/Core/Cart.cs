using ShoppeeEcommerce.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Cart
        : TrackableEntity<Guid>
    {
        public Guid? UserId { get; set; }
        public string? SessionId { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        public List<CartItem> CartItems { get; set; } = [];

        [NotMapped]
        public bool IsGuestCart => SessionId is not null;
        [NotMapped]
        public bool IsUser => UserId is not null;

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            var existing = GetItem(productId);

            if (existing is null)
            {
                CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, UnitPriceSnapshot = unitPrice });
            }
            else
            {
                existing.Increase(quantity);
            }
            Touch();
        }

        public void UpdateQuantity(Guid productId, int quantity)
        {
            var item = GetItem(productId);
            if (item != null)
            {
                item.SetQuantity(quantity);
                Touch();
            }
        }

        public void RemoveItem(Guid productId)
        {
            var item = GetItem(productId);
            if (item != null)
            {
                CartItems.Remove(item);
                Touch();
            }
        }

        public void Merge(Cart guestCart)
        {
            foreach (var item in guestCart.CartItems)
                AddItem(item.ProductId, item.Quantity, item.UnitPriceSnapshot);
            Touch();
        }

        public void MapToUser(Guid userId)
        {
            UserId = userId;
            SessionId = null;
            Touch();
        }

        public void Clear()
        {
            CartItems.Clear();
            Touch();
        }

        private CartItem? GetItem(Guid productId)
            => CartItems.FirstOrDefault(x => x.ProductId == productId);
        private void Touch(DateTime? timeStamp = null)
            => LastModifiedDate = timeStamp ?? DateTime.UtcNow;
    }
}
