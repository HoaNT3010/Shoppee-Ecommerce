using ShoppeeEcommerce.WebAPI.Domain.Abstractions;

namespace ShoppeeEcommerce.WebAPI.Domain.Entities.Base
{
    public abstract class TrackableEntity<TKey> : BaseEntity<TKey>, ITrackable
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
