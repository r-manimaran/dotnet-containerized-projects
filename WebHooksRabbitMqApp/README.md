# Webhooks system in .Net using Postgres in .Net Aspire

## Create Subscription with Webhook Url
![alt text](Images/image-2.png)

## Create Order

![alt text](Images/image-1.png)

![alt text](Images/image.png)

## Adding Persistance storage using Postgres 

**.Net Aspire Resources**
![alt text](Images/image3.png)



**Created tables with Schema**

- Access the PgAdmin tool using the resource in .Net Aspire

![alt text](Images/image4.png)

![alt text](Images/image5.png)

![alt text](Images/image6.png)

![alt text](Images/image7.png)

![alt text](Images/image8.png)

## Performance Issue if there is more number of subscriptions for the same event type.

- Here for creating a single order, the system is trying to trigger the web hook for all the subscriptions.
- As we are processing all the subscriptions one by one on a single Order Post request, it will delay the response.

![alt text](Images/image9.png)

![alt text](Images/image10.png)

- After implementiing Channel and Backgroundjob to process the Webhook dispatch, here are the traces in the Aspire dashboard. 
    - Here the Order creation post endpoint is in its own separate span and executed quickly.
    - Below spans are the webhook dispatcher process for each subscriptions.

![alt text](Images/image11.png)

## Added more OpenTelemetry traces to log the activity along with base Create Order

![alt text](Images/image12.png)

- in the Process Webhook, its processing the 3 Subscriptions I have in database.

![alt text](Images/image13.png)
