# Service Discovery using Consul
- Orders.Api
- Reporting.Api

- Orders.Api needs to call a endpoint in Reporting.Api using Http client
- Reporting.Api was registered with Consul for ServiceDiscovery.
- Use the ServiceDiscovery URL in the Orders.Api to call the Reporting.Api

![alt text](Images/image.png)

![alt text](Images/image-3.png)

- Consul Registration

![alt text](Images/image-1.png)

- Jaeger Traces

![alt text](Images/image-2.png)


![alt text](Images/image-4.png)
