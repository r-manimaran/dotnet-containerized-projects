# Modular Monolith Application with In-Memory MassTransit and Aspire Dashboard

This project demonstrates a modular monolithic application architecture using .NET, showcasing communication between Orders and Shipping modules through in-memory MassTransit messaging and public APIs.

## Architecture Overview

The application is structured as a modular monolith with the following components:

- **Orders Module**: Handles order creation and management
- **Shipping Module**: Processes shipping requests for created orders
- **In-Memory MassTransit**: Manages inter-module communication
- **PostgreSQL**: Stores order and shipping data
- **Aspire Dashboard**: Provides observability and monitoring

![alt text](Images/image-9.png)


## Tech Stack

- .NET 8+
- PostgreSQL 17
- MassTransit (In-Memory)
- OpenTelemetry
- Docker & Docker Compose
- Entity Framework Core
- Swagger/OpenAPI
- Aspire Dashboard

## Service Communication
The services communicate through:
1. Public API interfaces
2. MassTransit message contracts
3. In-memory message bus

### Message Flow

![alt text](Images/image-10.png)


![alt text](Images/image.png)

- Create a new Order

![alt text](Images/image-1.png)

- Structured Logs

![alt text](Images/image-4.png)

![alt text](Images/image-5.png)

![alt text](image-6.png)

![alt text](Images/image-7.png)

- Trace

![alt text](Images/image-2.png)

![alt text](Images/image-3.png)

![alt text](Images/image-8.png)