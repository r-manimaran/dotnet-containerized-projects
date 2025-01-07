using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Confguration;

public abstract class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        // Base Entity Properties
        builder.Property(p => p.CreatedOn)
               .IsRequired();

        builder.Property(p => p.ModifiedOn)
               .IsRequired();

        builder.Property(p => p.CreatedBy)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(p => p.ModifiedBy)
               .IsRequired()
               .HasMaxLength(50);
    }
}
