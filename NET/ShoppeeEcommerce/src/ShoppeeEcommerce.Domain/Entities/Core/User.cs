using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class User : BaseEntity<Guid>
    {
        public string? NormalizedUserName { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? PhoneNumber { get; set; }

        public ICollection<Category> CreatedCategories { get; set; } = [];
        public ICollection<Product> CreatedProducts { get; set; } = [];
        public ICollection<ProductRating> ProductRatings { get; set; } = [];
    }
}
