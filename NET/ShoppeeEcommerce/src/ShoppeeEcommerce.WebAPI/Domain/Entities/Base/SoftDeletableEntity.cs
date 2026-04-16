using ShoppeeEcommerce.WebAPI.Domain.Abstractions;

namespace ShoppeeEcommerce.WebAPI.Domain.Entities.Base
{
    public abstract class SoftDeletableEntity<TKey>
        : TrackableEntity<TKey>, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
