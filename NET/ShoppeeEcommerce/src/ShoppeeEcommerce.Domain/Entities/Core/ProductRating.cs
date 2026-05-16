using ShoppeeEcommerce.Domain.Entities.Base;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class ProductRating
        : TrackableEntity<Guid>
    {
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public int HelpfulCount { get; set; }
        public RatingStatus Status { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public Guid CreatorId { get; set; }
        public User? Creator { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid? OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }
    }
}
