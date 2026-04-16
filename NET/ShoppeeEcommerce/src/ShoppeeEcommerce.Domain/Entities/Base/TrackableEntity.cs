using ShoppeeEcommerce.Domain.Abstractions;

namespace ShoppeeEcommerce.Domain.Entities.Base
{
    public abstract class TrackableEntity<TKey> : BaseEntity<TKey>, ITrackable
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
