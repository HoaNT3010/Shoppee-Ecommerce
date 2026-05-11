using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Persistence.Configurations
{
    internal class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        const string TableName = "Payments";

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable(TableName, DbSchema.Core);

            builder.Property(p => p.StripePaymentIntentId)
                .HasMaxLength(255);
            builder.Property(p => p.StripeClientSecret)
                .HasMaxLength(500);
            builder.Property(p => p.StripeChargeId)
                .HasMaxLength(255);
            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.RefundedAmount)
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");
            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            // Indexes for common lookup
            // Payment intent ID for webhook handling
            builder.HasIndex(p => p.StripePaymentIntentId)
                .IsUnique();
            // Query user's payments
            builder.HasIndex(p => p.UserId);
            // Query order's payment
            builder.HasIndex(p => p.OrderId);
            // Filter by status
            builder.HasIndex(p => p.Status);

            // Payment - Order: 1-to-1
            // One order has at most one active payment at a time
            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            // Payment - User: M-to-1
            // A user can have many payments over their lifetime
            builder.HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
