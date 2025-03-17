using Aspire.eshop.CatalogDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.eshop.CatalogDb.Data.Configuration;

public class CatalogTypeConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .UseHiLo("catalog_type_hilo")
               .IsRequired();

        builder.Property(cb => cb.Type)
                .IsRequired()
                .HasMaxLength(100);
    }
}
