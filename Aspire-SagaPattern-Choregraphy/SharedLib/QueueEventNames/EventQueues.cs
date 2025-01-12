namespace SharedLib.QueueEventNames;

public static class EventQueues
{

    //Inventory
    public const string StockEnoughEvent_QueueName = "StockEnough";
    public static readonly string StoclNotEnoughEvent_QueueName = "StockNotEnough";

    // Basket Api
    public const string BasketConfirmationEvent_QueueName = "BasketConfirmed";
    public const string BasketCancelEvent_QueueName = "BacketCancel";

    // Order Api
    public const string OrderCreatedEvent_QueueName = "OrderCreated";
    public const string OrderPaymentRequestSentEvent_QueueName = "OrderPaymentRequest";

    public const string OrderConfirmationEmailSentEvent_QueueName = "OrderConfirmedEmail";

    //Payment
    public const string PaymentCompletedEvent_QueueName = "PaymentCompleted";
    public const string PaymentFailedEvent_QueueName = "PaymentFailed";



}
