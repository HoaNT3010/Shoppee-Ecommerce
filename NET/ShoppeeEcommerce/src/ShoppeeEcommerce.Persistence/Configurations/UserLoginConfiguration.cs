using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        const string TableName = "UserLogins";
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
        }
    }
}
