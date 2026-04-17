using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        const string TableName = "Roles";
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
