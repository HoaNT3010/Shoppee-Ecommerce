using ShoppeeEcommerce.Domain.Abstractions;

namespace ShoppeeEcommerce.Domain.Entities.Base
{
    public abstract class SoftDeletableEntity<TKey>
        : TrackableEntity<TKey>, ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual void SoftDelete(DateTime? deleteTimestamp = null)
        {
            IsDeleted = true;
            DeletedDate = deleteTimestamp ?? DateTime.UtcNow;
        }

        public virtual void RestoreSoftDelete()
        {
            IsDeleted = false;
            DeletedDate = null;
        }
    }
}
