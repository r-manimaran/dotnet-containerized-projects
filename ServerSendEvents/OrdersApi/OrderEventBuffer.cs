using OrdersApi.Models;
using System.Collections.Concurrent;
using System.Net.ServerSentEvents;

namespace OrdersApi;

public class OrderEventBuffer(int maxBufferSize = 100)
{
    private readonly ConcurrentQueue<SseItem<OrderEvent>> _buffer = new();
    private long _nextEventId = 1;

    public SseItem<OrderEvent> Add(OrderEvent order)
    {
        var eventId = Interlocked.Increment(ref _nextEventId)-1;
        var sseItem = new SseItem<OrderEvent>(order)
        {
            EventId = eventId.ToString()
        };
        _buffer.Enqueue(sseItem);

        // Maintain buffer size
        while(_buffer.Count > maxBufferSize)
        {
            _buffer.TryDequeue(out _);
        }
        return sseItem;
    }

    public IEnumerable<SseItem<OrderEvent>> GetEventsAfter(string? lastEventId)
    {
        
        if (long.TryParse(lastEventId, out var lastId))
        {
            return _buffer.Where(e => long.Parse(e.EventId) > lastId);
        }
        return _buffer;

    }
}
