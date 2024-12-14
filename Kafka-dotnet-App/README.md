## Using Confluent Kafka in .Net Application to Produce and Consume messages

- Running the Kakfa in the docker. Created the docker-compose file with the Kafka services.
- Execute docker-compose up -d on the prompt window to start the containers.
- If the images are not there, it will download the images and spin up the containers.

![alt text](images/image.png)

**Test the Control center**:
![alt text](images/image-1.png)

Check the Topics:Our new-order-topic is not yet created.
![alt text](images/image-2.png)

Lets run the Order's API: Create a new Order

![alt text](images/image-3.png)

Now the Order topic will be created in Kafka as shown below.

![alt text](images/image-4.png)

## Consumer

![alt text](images/image-5.png)

![alt text](images/image-6.png)

![alt text](images/image-7.png)

![alt text](images/image-8.png)

![alt text](images/image-9.png)

![alt text](images/image-10.png)

![alt text](images/image-11.png)