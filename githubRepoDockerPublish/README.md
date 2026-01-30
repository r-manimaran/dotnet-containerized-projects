
# Use dotnet Publish to create container image

```bash
dotnet publish --os linux -t:PublishContainer

docker images

docker run -d -p 3500:8080 productsapi

curl http://localhost:3500/products

```

![alt text](image.png)

![alt text](image-1.png)