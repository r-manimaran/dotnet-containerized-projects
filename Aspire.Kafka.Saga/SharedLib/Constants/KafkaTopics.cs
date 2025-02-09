using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Enums;

public static class KafkaTopics 
{
    public const string ORDER_CREATED = "order-created";
    public const string PRODUCTS_RESERVED = "products-reserved";
    public const string PAYMENT_PROCESSED = "payment-processed";

    public const string PRODUCTS_RESERVATION_FAILED = "products-reservation-failed";
    public const string PRODUCTS_RESERVATION_CANCELED = "products-reservation-canceled";
        
    public const string PAYMENT_FAILED = "payment-failed";
   
}
