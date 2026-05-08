using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        const string TableName = "CartItems";
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(ci => ci.ProductId)
                .IsRequired();
            builder.Property(ci => ci.UnitPriceSnapshot)
                .HasPrecision(18, 2);

            builder.HasIndex(ci => ci.ProductId);
        }
    }
}
