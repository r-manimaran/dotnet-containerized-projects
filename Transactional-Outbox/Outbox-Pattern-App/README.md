## Implementing Outbox transaction pattern in .net web api, MassTransit, RabbitMQ

**Architecture Flow:**
![alt text](images/image-10.png)


### Without Outbox pattern
 - In this approach the Orders.Api will publish a message when new order is placed and stored in SQL Server table.
 - Using MassTrasit, the message get published to RabbitMQ
 - In the Shipping.Api, we have a consumer configured which will consume the message and create a shipment entry in the SQLite database.
 - RabbitMQ is running in docker container
 - I'm running the other API's in the command prompt using dotnet run command. (Separate command prompt for both the API)

 ![alt text](images/image-1.png)

 ![alt text](images/image.png)

 ![alt text](images/image-2.png)

 ![alt text](images/image-6.png)

 ![alt text](images/image-3.png)

 ![alt text](images/image-4.png)


 ![alt text](images/image-5.png)

 **Implementing Outbox message**
![alt text](images/image-8.png)

![alt text](images/image-9.png)

 ![alt text](images/image-7.png)