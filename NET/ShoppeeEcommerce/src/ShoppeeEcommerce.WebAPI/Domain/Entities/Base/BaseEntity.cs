using ShoppeeEcommerce.WebAPI.Domain.Abstractions;

namespace ShoppeeEcommerce.WebAPI.Domain.Entities.Base
{
    public abstract class BaseEntity<TKey> : IHasKey<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
