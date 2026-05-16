using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class ProductRatingSummaryConfiguration
        : IEntityTypeConfiguration<ProductRatingSummary>
    {
        const string TableName = "ProductRatingSummaries";
        public void Configure(EntityTypeBuilder<ProductRatingSummary> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);
            builder.Property(p => p.TotalSum)
                .HasPrecision(9, 1);
            builder.HasQueryFilter(pr => !pr.Product!.IsDeleted);
        }
    }
}
