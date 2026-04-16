using ShoppeeEcommerce.Domain.Abstractions;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Product : SoftDeletableEntity<Guid>, IHasCreator<Guid?>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Guid? CreatedBy { get; set; }
        public List<ProductImage> ProductImages { get; set; } = [];
        public ICollection<ProductRating> ProductRatings { get; set; } = [];
    }
}
