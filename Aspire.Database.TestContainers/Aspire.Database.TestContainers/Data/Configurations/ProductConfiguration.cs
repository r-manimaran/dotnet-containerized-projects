using Aspire.Database.TestContainers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspire.Database.TestContainers.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x=>x.Id);

        builder.Property(x=>x.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x=>x.Price)
               .IsRequired()
               .HasPrecision(6,2);
    }
}
