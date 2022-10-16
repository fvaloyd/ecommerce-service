using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.BrandId).IsRequired();
        builder.Property(p => p.CategoryId).IsRequired();
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Price).IsRequired();
    }
}
