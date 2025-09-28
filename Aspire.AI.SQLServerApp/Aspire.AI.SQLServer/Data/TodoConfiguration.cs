using Aspire.AI.SQLServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspire.AI.SQLServer.Data;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {

        builder.OwnsOne(t => t.Metadata, b =>
        {
            b.ToJson(); // Store as JSON column
            b.OwnsOne(m => m.CreatedBy);
        });


        //builder.Property(u => u.Metadata)
        //   .HasColumnType("json");
    }
}
