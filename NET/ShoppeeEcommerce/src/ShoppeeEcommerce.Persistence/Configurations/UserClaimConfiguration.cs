using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        const string TableName = "UserClaims";
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
