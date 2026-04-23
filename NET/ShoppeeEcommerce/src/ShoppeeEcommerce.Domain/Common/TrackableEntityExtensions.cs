using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Common
{
    public static class TrackableEntityExtensions
    {
        public static void SetUpdatedDateTime<TKey>(
            this TrackableEntity<TKey> trackableEntity,
            DateTime? updatedDateTime = null)
        {
            trackableEntity.UpdatedDate = updatedDateTime ?? DateTime.UtcNow;
        }
    }
}
