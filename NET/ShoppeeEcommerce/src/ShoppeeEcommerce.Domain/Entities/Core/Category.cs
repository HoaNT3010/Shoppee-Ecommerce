using ShoppeeEcommerce.Domain.Abstractions;
using ShoppeeEcommerce.Domain.Entities.Base;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Category
        : SoftDeletableEntity<Guid>, IHasCreator<Guid?>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid? CreatorId { get; set; }
        public User? Creator { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
