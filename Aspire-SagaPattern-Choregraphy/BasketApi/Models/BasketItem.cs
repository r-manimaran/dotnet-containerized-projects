using SharedLib.Enums;
using SharedLib.Models;

namespace BasketApi.Models;
    public class BasketItem :EntityBase
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public BasketStatus Status { get; set; }
        public BasketItem(int productId, int basketId, decimal price, int count, BasketStatus status)
        { 
            ProductId = productId;
            BasketId = basketId;
            Price = price;
            Status = status;
            Count = count;
        }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

    }

