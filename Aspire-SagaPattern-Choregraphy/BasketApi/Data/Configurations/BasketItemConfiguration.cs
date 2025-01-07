using BasketApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Confguration;

namespace BasketApi.Data.Configurations;

public class BasketItemConfiguration : EntityBaseConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        base.Configure(builder);
        
        // Primary Key
        builder.HasKey(bi => bi.Id);

        // Properties
        builder.Property(bi => bi.ProductId)
               .IsRequired();

        builder.Property(bi=>bi.Count)
               .IsRequired()
               .HasDefaultValue(1);
        
        builder.Property(bi=>bi.Price)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(bi=>bi.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(bi=>bi.BasketId)
               .IsRequired();

        // Indexes
        builder.HasIndex(bi => bi.BasketId);
        builder.HasIndex(bi => bi.ProductId);

        // Table Name optional
        builder.ToTable("BasketItems");

    }
}
