using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        const string TableName = "Orders";
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            // For user orders listing
            builder.HasIndex(o => new { o.UserId, o.Status, o.CreatedDate });
            // For admin orders listing
            builder.HasIndex(o => new { o.Status, o.CreatedDate });

        }
    }
}
