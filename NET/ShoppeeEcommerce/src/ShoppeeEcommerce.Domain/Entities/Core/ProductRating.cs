using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class ProductRating
        : TrackableEntity<Guid>
    {
        public int Stars { get; set; }
        public string? Comment { get; set; }

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
