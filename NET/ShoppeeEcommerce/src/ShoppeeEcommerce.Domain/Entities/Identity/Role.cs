using Microsoft.AspNetCore.Identity;

namespace ShoppeeEcommerce.Domain.Entities.Identity
{
    public sealed class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
