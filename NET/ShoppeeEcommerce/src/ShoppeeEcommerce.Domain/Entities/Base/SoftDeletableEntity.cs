using ShoppeeEcommerce.Domain.Abstractions;

namespace ShoppeeEcommerce.Domain.Entities.Base
{
    public abstract class SoftDeletableEntity<TKey>
        : TrackableEntity<TKey>, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
