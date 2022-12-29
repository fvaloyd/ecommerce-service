using Ecommerce.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.PaymentTransactionId).IsRequired();

        builder.Property(o => o.Total).HasColumnType("decimal(18,4)").IsRequired();

        builder.Property(o => o.ApplicationUserId).IsRequired();

        builder.Property(o => o.OrderDate).IsRequired();
    }
}