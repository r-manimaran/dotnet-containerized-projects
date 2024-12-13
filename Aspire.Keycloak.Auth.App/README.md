## Running KeyCloak in .Net Aspire and Authenticate Web API endpoints with Postgres database


## KeyCloak Running in .Net Aspire as Container
![alt text](image.png)

## Added Postgres using Aspire as Container with stocks as the database.
![alt text](image-6.png)

* Login in to Keycloak and created a realm named maransys
* Added a client and some test users with credentials.

## Running the endpoint without Authentication enabled
![alt text](image-1.png)

Get the access and Refresh token from Keycloak using postman
![alt text](image-2.png)

## Call API endpoint without Authorization Token
![alt text](image-3.png)

- Now pass the access token which we got from the Keycloak to the request.

![alt text](image-4.png)

## Traces in .Net Aspire
![alt text](image-5.png)
