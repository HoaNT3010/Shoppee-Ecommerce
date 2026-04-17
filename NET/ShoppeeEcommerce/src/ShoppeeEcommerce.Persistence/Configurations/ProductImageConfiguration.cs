using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        const string TableName = "ProductImages";
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);
            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);
        }
    }
}
