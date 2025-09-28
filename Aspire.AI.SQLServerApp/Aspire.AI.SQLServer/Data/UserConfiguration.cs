using Aspire.AI.SQLServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspire.AI.SQLServer.Data;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(t => t.Profile, b =>
        {
            b.ToJson("Profile");
            b.OwnsOne(p => p.Details, d =>
            {
                d.OwnsOne(det => det.Address);
            });
        });
        // builder.ComplexProperty(b => b.Profile, b => b.ToJson());

        //builder.Property(u => u.Profile)
        //    .HasColumnType("json");
    }
}
