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
            builder.Property(pr => pr.Comment)
                .HasMaxLength(1000);
            builder.HasOne(pr => pr.User)
                .WithMany(u => u.ProductRatings)
                .HasForeignKey(pr => pr.UserId);
            builder.HasOne(pr => pr.Product)
                .WithMany(p => p.ProductRatings)
                .HasForeignKey(pr => pr.ProductId);
        }
    }
}
