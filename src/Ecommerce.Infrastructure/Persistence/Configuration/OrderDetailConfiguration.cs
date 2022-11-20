using System.Runtime.Serialization;
using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configuration;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(od => new {od.ProductId, od.OrderId});
        builder.Property(od => od.ProductId).IsRequired();
        builder.Property(od => od.Quantity).IsRequired();
        builder.Property(od => od.OrderId).IsRequired();
        builder.Property(od => od.ApplicationUserId).IsRequired();
        builder.Property(od => od.UnitPrice).IsRequired();
    }
}