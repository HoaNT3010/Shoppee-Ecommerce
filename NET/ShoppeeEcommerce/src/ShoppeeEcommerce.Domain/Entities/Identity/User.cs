using Microsoft.AspNetCore.Identity;
using ShoppeeEcommerce.Domain.Abstractions;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Domain.Entities.Identity
{
    public sealed class User : IdentityUser<Guid>, ITrackable
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Category> CreatedCategories { get; set; } = [];
        public ICollection<Product> CreatedProducts { get; set; } = [];
        public ICollection<ProductRating> ProductRatings { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Payment> Payments { get; set; } = [];

        public void MarkEntityAsUpdated(DateTime? updateTimestamp = null)
        {
            UpdatedDate = updateTimestamp ?? DateTime.UtcNow;
        }
    }
}
