using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerApi.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x=>x.Id);

            builder.Property(x=>x.Name).IsRequired().HasMaxLength(50);

            builder.Property(x=>x.Email).IsRequired().HasMaxLength(50);
        }
    }
}
