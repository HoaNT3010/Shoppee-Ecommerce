using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        const string TableName = "Categories";
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);
            builder.Property(c => c.Name)
                .HasMaxLength(100);
            builder.Property(c => c.Description)
                .HasMaxLength(1000);
            builder.HasOne(c => c.Creator)
                .WithMany(u => u.CreatedCategories)
                .HasForeignKey(c => c.CreatorId)
                .IsRequired(false);
        }
    }
}
