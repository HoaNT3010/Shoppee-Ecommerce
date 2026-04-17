using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Domain.Entities.Identity
{
    public sealed class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<Category> CreatedCategories { get; set; } = [];
        public ICollection<Product> CreatedProducts { get; set; } = [];
        public ICollection<ProductRating> ProductRatings { get; set; } = [];
    }
}
