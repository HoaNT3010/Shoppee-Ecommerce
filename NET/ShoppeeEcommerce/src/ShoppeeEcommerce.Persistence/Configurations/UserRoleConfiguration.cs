using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        const string TableName = "UserRoles";
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
