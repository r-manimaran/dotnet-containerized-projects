# Containerize and Orchestrate .Net API using Docker and Kubernates

1. Create a .Net Web API Project
2. Create a Docker file and mapping

```bash
 # Build the Docker file
 > docker build .
```
Successfully build and created the Image

![alt text](image.png)

```bash
# Build the Docker file and tag with a name
> docker build . -t blogapik8s
```

```bash
# Run the Docker image in the docker desktop
> docker run -p 8082:80 -e ASPNETCORE_URLS=http://+:80 blogapik8s
```

Now Access the Endpoints from the service running in Docker.
![alt text](image-1.png)