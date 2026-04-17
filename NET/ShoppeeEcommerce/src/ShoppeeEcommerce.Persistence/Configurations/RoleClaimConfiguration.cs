using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        const string TableName = "RoleClaims";
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
