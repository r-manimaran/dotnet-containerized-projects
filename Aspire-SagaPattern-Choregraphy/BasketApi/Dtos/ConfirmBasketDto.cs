namespace BasketApi.Dtos;
    public class ConfirmBasketDto
    {
        public int CustomerId { get; set; }
        public List<BasketItemDto> BasketItems { get; set; }
        public PaymentDto Payment { get; set; }
    }

