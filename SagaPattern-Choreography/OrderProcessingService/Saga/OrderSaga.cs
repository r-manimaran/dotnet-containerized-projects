using OrderProcessingService.Models;
using OrderProcessingService.Services;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

namespace OrderProcessingService.Saga;

public class OrderSaga : Saga<OrderSagaData>,
                        IAmInitiatedBy<PlaceOrderCommand>,
                        IHandleMessages<ProcessPaymentCommand>,
                        IHandleMessages<ShipOrderCommand>,
                        IHandleMessages<OrderShippedEvent>

{
    private readonly IBus _bus;
    private readonly IOrderRepository _orderRepository;

    public OrderSaga(IBus bus, IOrderRepository orderRepository)
    {
        _bus = bus;
        _orderRepository = orderRepository;
    }

  

    protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
    {
        config.Correlate<PlaceOrderCommand>(m => m.OrderId, d => d.OrderId);
        config.Correlate<ProcessPaymentCommand>(m=>m.OrderId, d => d.OrderId);
        config.Correlate<ShipOrderCommand>(m => m.OrderId, d => d.OrderId);
        config.Correlate<OrderShippedEvent>(m=>m.OrderId, d=>d.OrderId);
    }

    // Implementation for Place Order Command
    public Task Handle(PlaceOrderCommand message)
    {
        Data.OrderId = message.OrderId;
        Data.IsOrderPlaced = true;

        _orderRepository.AddOrder(new()
        {
            OrderId = message.OrderId,
            Status = OrderStatus.Placed
        });
        return Task.CompletedTask;
    }


    // Implementation for ProcessPaymentCommand
    public async Task Handle(ProcessPaymentCommand message)
    {
       Data.IsPaymentProcessed = true;
       
        var order = _orderRepository.GetOrderById(message.OrderId);
        order.Status = OrderStatus.Processing;

        await _bus.Send(new ShipOrderCommand { OrderId = message.OrderId, });
    }

    public async Task Handle(ShipOrderCommand message)
    {
        Data.IsOrderShipped = true;
        await _bus.Send(new OrderShippedEvent {  OrderId = Data.OrderId });
    }

    public Task Handle(OrderShippedEvent message)
    {
        var order = _orderRepository.GetOrderById(message.OrderId);
        order.Status = OrderStatus.Completed;

        MarkAsComplete();
        return Task.CompletedTask;
    }
}
