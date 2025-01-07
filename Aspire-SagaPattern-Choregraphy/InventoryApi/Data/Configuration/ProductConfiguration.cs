using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Confguration;

namespace InventoryApi.Data.Configuration;

public class ProductConfiguration : EntityBaseConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        // Primary Key
        builder.HasKey(p=>p.Id);

        // Product Specific properties
        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(p=>p.Price)
               .IsRequired()
               .HasPrecision(18,2);

        builder.Property(p=>p.Quantity)
               .IsRequired();

        // create a index on the Product Name
        builder.HasIndex(p => p.Name)
               .IsUnique();
      
    }
}
