using Orders.Api.Models;
using System.Text.Json;

namespace Orders.Api
{
    public static class OutboxExtensions
    {
        internal static async Task InsertOutboxMessage<T>(
            this AppDbContext dbContext,
            T message) where T : notnull
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = message.GetType().FullName!,
                Content = JsonSerializer.Serialize(message),
                OccuredOnUtc = DateTime.UtcNow
            };

            await dbContext.OutboxMessages.AddAsync(outboxMessage);
            dbContext.SaveChanges();

        }
    }
}
