using ShoppeeEcommerce.Domain.Entities.Base;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class ProductRating
        : TrackableEntity<Guid>
    {
        public int Stars { get; set; }
        public string? Comment { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
