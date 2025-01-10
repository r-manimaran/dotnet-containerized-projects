using BasketApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Confguration;
using Microsoft.EntityFrameworkCore;

namespace BasketApi.Data.Configurations;

public class BasketConfiguration : EntityBaseConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        base.Configure(builder);
        
        // primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x=>x.CustomerId)
            .IsRequired();

        builder.Property(b=>b.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x=>x.TotalPrice)
            .IsRequired()
            .HasDefaultValue(0)
            .HasPrecision(18, 2)
            .HasColumnType("decimal(18,2)");;

        // Relationships
        builder.HasMany(x => x.BasketItems)
            .WithOne(x => x.Basket)
            .HasForeignKey(x => x.BasketId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(b=>b.CustomerId); 
        builder.HasIndex(b=>b.Status);

        // Table name (optional)
        builder.ToTable("Baskets");
    }
}
