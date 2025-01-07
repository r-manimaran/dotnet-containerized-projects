using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentApi.Models;
using SharedLib.Confguration;
using SharedLib.Enums;

namespace PaymentApi.Data.Configuration;

public class PaymentConfiguration : EntityBaseConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        base.Configure(builder);
        
        // Configure Primary Key
        builder.HasKey(p=>p.Id);

        // Configure Properties
        builder.Property(p=>p.OrderId)
               .IsRequired();

        builder.Property(p=>p.CardNumber)
               .IsRequired()
               .HasMaxLength(16);

        builder.Property(p=>p.CardHolderName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(p=>p.Expiration)
               .IsRequired()
               .HasMaxLength(5);
        
        builder.Property(p=>p.CVV)
               .IsRequired()
               .HasMaxLength(3);

        builder.Property(p=>p.CVV)
               .IsRequired()
               .HasMaxLength(3);
        
         builder.Property(p=>p.Amount)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(p=>p.Status)
               .IsRequired()
               .HasDefaultValue(PaymentStatus.Pending);
        
        builder.Property(p=>p.CustomerId)
               .IsRequired();

        // Create Indexes
        builder.HasIndex(p=>p.OrderId)
               .IsUnique();
    }
}
