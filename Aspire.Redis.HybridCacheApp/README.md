## HybridCache with Redis in Aspire .Net

![alt text](Images/image.png)

1. First call hits the httpendpoints and get the results
2. When calling second time, it will bring it from the IMemoryCache

Here are the distributed Traces for the calls.

![alt text](Images/image-1.png)

![alt text](Images/image-2.png)

- IMemory cache is limited to single server.


With Invalidate Cache Options
![alt text](Images/image-3.png)

Introduce Redis using .Net Aspire and Add DistributedCache in our app
![alt text](Images/image-4.png)

![alt text](Images/image-5.png)

![alt text](Images/image-6.png)

Now Stop our API service and continue to run the Redis. This action will remove the cache information from the local cache. As Redis is running, the data will persist there.
![alt text](Images/image-7.png)

Now Start and test the endpoint. 

![alt text](Images/image-8.png)


## Concurrent call to Hybrid cache.

- Call concurrently to the Hybrid cache API.
- It will cache in first request and subsequent call go to the cache.

![alt text](Images/image-9.png)
