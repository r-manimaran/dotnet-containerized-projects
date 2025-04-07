# YARP running in Container



## Troubleshooting
- To verify the hostnames are resolvable witin the docker network.
```
docker exec yarp-container ping productsapi
```

- Check the Docker Compose logs
```
> docker-compose logs -f
```