using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class OrderItemConfiguration
        : IEntityTypeConfiguration<OrderItem>
    {
        const string TableName = "OrderItems";
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(oi => oi.ProductName)
                .HasMaxLength(200);
            builder.Property(oi => oi.ProductSKU)
                .HasMaxLength(128);
            builder.Property(oi => oi.PriceSnapshot)
                .HasPrecision(18, 2);
        }
    }
}
