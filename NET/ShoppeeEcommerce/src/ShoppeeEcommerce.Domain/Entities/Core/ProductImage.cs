using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class ProductImage : BaseEntity<int>
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
