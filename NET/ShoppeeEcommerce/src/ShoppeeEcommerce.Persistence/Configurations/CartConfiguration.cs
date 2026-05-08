using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        const string TableName = "Carts";
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(c => c.SessionId)
                .HasMaxLength(255);

            builder.HasMany(c => c.CartItems)
               .WithOne(ci => ci.Cart)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.SessionId);
        }
    }
}
