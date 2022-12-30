using Ecommerce.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configuration;

public class ProductStoreConfiguration : IEntityTypeConfiguration<ProductStore>
{
    public void Configure(EntityTypeBuilder<ProductStore> builder)
    {
        builder.HasKey(ps => new {ps.ProductId, ps.StoreId});
        builder.Property(ps => ps.ProductId).IsRequired();
        builder.Property(ps => ps.StoreId).IsRequired();
    }
}