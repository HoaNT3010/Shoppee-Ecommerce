using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
    {
        const string TableName = "ProductRatings";
        public void Configure(EntityTypeBuilder<ProductRating> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(pr => pr.Title)
                .HasMaxLength(150);
            builder.Property(pr => pr.Comment)
                .HasMaxLength(2000);

            builder.HasQueryFilter(pr => !pr.Product!.IsDeleted);

            builder.HasOne(pr => pr.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pr => pr.Creator)
                .WithMany(u => u.ProductRatings)
                .HasForeignKey(pr => pr.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pr => pr.OrderItem)
                .WithOne()
                .HasForeignKey<ProductRating>(r => r.OrderItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasIndex(r => new { r.ProductId, r.CreatorId }).IsUnique();
            // Fast lookup of all ratings for a product
            builder.HasIndex(r => r.ProductId);
            // Fast lookup of all ratings by a user
            builder.HasIndex(r => r.CreatorId);
        }
    }
}
