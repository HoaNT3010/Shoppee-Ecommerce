using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        const string TableName = "UserTokens";
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
