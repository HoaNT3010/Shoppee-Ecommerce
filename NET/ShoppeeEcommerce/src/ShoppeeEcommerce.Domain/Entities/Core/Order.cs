using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Base;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Order
        : TrackableEntity<Guid>
    {
        // No reference to user
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
        public Payment? Payment { get; set; }

        public void RecalculateTotal()
        {
            TotalPrice = Items.Sum(i => i.SubTotal);
        }

        public static Order CreateNewOrder(Guid userId)
        {
            return new Order()
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalPrice = 0,
            };
        }

        public void CancelOrder(DateTime? cancelDate = null)
        {
            Status = OrderStatus.Cancelled;
            this.SetUpdatedDateTime(cancelDate);
        }

        public void MarkAsPaid(DateTime? paidDate = null)
        {
            Status = OrderStatus.Paid;
            this.SetUpdatedDateTime(paidDate);
        }

        public void MarkAsRefunded(DateTime? refundDate = null)
        {
            Status = OrderStatus.Refunded;
            this.SetUpdatedDateTime(refundDate);
        }

        public void MarkAsAwaitingPayment(DateTime? awaitDate = null)
        {
            Status = OrderStatus.AwaitingPayment;
            this.SetUpdatedDateTime(awaitDate);
        }
    }
}
