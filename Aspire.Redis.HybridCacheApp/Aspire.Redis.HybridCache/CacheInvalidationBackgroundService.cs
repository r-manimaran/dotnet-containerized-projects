using Microsoft.Extensions.Caching.Hybrid;
using StackExchange.Redis;

internal sealed class CacheInvalidationBackgroundService(
                        IConnectionMultiplexer connectionMultiplexer,
                        HybridCache hybridCache) : BackgroundService
{
    public const string Channel = "cache-invalidation";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = connectionMultiplexer.GetSubscriber();

        await subscriber.SubscribeAsync(RedisChannel.Literal(Channel), (_, key) =>
        {
            var task = hybridCache.RemoveAsync(key);

            if (!task.IsCompleted)
            {
                task.GetAwaiter().GetResult();
            }
        });
    }

}


