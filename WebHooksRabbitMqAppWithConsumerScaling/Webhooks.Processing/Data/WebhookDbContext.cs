using Microsoft.EntityFrameworkCore;
using Orders.Api.Models;

namespace Webhooks.Processing.Data;

internal sealed class WebhookDbContext : DbContext
{
    public WebhookDbContext(DbContextOptions<WebhookDbContext> options) :base(options)
    {
        
    }


    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }

    public DbSet<WebhookDeliveryAttempt> WebhookDeliveryAttempts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<WebhookSubscription>(builder =>
        {
            builder.ToTable("subscriptions", "webhooks");
            builder.HasKey(o => o.Id);
        });

        modelBuilder.Entity<WebhookDeliveryAttempt>(builder =>
        {
            builder.ToTable("delivery_attempts", "webhooks");
            builder.HasKey(o => o.Id);

            // Foreign Key to subscription table
            builder.HasOne<WebhookSubscription>()
                .WithMany()
                .HasForeignKey(d => d.WebhookSubscriptionId);

        });
    }
        
}
