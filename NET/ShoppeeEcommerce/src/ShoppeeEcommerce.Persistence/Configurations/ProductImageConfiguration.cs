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

            builder.Property(pi => pi.AltText)
                .HasMaxLength(255);
            builder.HasIndex(pi => pi.ProductId)
                .HasFilter("[IsMain] = 1")
                .IsUnique();
            builder.HasIndex(pi => new { pi.ProductId, pi.DisplayOrder })
                .IsUnique();
            builder.Property(pi => pi.PublicId)
                .IsRequired()
                .HasMaxLength(512);
            builder.HasQueryFilter(pi => !pi.Product.IsDeleted);
        }
    }
}
