using SharedLib.Sagas.Messages;

namespace SharedLib.Sagas.Events;

// Basket Api


// BasketCancelEvent
public record BasketConfirmedEvent(int BasketId, int CustomerId, PaymentMessage PaymentMessage, List<BasketItemMessage> BasketItemMessages);


// Inventory Api Events

public record OrderCreatedEvent(Guid OrderId);

public record OrderConfirmationEmailSent(Guid OrderId);

public record OrderPaymentRequestSent(Guid OrderId);

