using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        const string TableName = "Products";
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);
            builder.Property(p => p.Name)
                .HasMaxLength(200);
            builder.Property(p => p.Description)
                .HasMaxLength(1000);
            builder.HasOne(p => p.Creator)
                .WithMany(u => u.CreatedProducts)
                .HasForeignKey(p => p.CreatorId)
                .IsRequired(false);
            // Many-to-many with Category
            builder.HasMany(p => p.Categories)
                .WithMany(c => c.Products);

            builder.HasMany(p => p.ProductImages)
               .WithOne(pi => pi.Product)
               .HasForeignKey(pi => pi.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.ProductRatings)
               .WithOne(pr => pr.Product)
               .HasForeignKey(pr => pr.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(128);
            builder.HasIndex(p => p.Name)
                .IsUnique();
            builder.HasIndex(p => p.SKU)
                .IsUnique();
            builder.Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}
