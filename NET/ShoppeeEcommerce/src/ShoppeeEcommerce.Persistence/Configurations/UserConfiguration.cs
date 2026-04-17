using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        const string TableName = "Users";
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableName, DbSchema.Identity);
            builder.Property(x => x.FirstName)
                .HasMaxLength(100);
            builder.Property(x => x.LastName)
                .HasMaxLength(100);
        }
    }
}
