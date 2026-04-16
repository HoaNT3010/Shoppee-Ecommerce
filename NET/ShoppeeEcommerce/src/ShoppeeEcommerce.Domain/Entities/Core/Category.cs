using ShoppeeEcommerce.Domain.Abstractions;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Category
        : SoftDeletableEntity<Guid>, IHasCreator<Guid?>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid? CreatedBy { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
