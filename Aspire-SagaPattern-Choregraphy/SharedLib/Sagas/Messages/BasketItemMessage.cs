using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Sagas.Messages
{
    public class BasketItemMessage
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int BasketId { get; set; }
        public BasketItemMessage(int productId, int count, decimal price, int basketId)
        {
            ProductId = productId;
            Count = count;
            Price = price;
            BasketId = basketId;
        }
    }
}
