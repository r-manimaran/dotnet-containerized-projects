
### Installing .Net Aspir8
```bash
> dotnet tool install -g aspirate --prerelease
```
If it is already installed installed, you can do the update.
```bash
> dotnet tool update -g aspirate --prerelease
```
## Aspirate operations

![alt text](Images/image-1.png)

### Initialize the aspirate

![alt text](Images/image.png)

- Set docker.io for container registry fallback value
- Set the username of your docker hub registry value.

- After setting the configuration, the init command will create a json file in the AppHost project. Here the file is aspirate.json

```json
    {
  "TemplatePath": null,
  "ContainerSettings": {
    "Registry": "docker.io",
    "RepositoryPrefix": "rmanimaran",
    "Tags": [
      "latest"
    ],
    "Builder": "docker"
  }
}
```
## Build the Images
- Run the command `aspirate generate` to build the images and create the Kubernetes manifest files for the projects (Api and RabbitMQ)
- Enter the password for the secrets in the kubernetes

 ![alt text](Images/image-2.png)

 ![alt text](Images/image-3.png)

 ![alt text](Images/image-4.png)

 - confirm Y to install the aspire dashboard
 ![alt text](Images/image-5.png)

 ![alt text](Images/image-6.png)

 ![alt text](Images/image-7.png)

![alt text](Images/image-8.png)

![alt text](Images/image-9.png)

## Check the Kubernetes Environment
![alt text](Images/image-10.png)

## Deploy the application
- Run the command `aspirate apply` to apply and deploy the generated manifests to a Kubernetes cluster.

![alt text](Images/image-11.png)

![alt text](Images/image-12.png)

![alt text](Images/image-13.png)

![alt text](Images/image-14.png)

![alt text](Images/image-15.png)

## Deployed application in docker

![alt text](Images/image-16.png)

![alt text](Images/image-17.png)

![alt text](Images/image-18.png)

![alt text](Images/image-19.png)

## Access the application
![alt text](Images/image-20.png)

![alt text](Images/image-21.png)

![alt text](Images/image-22.png)

- port forward Dashboard

![alt text](Images/image-23.png)

![alt text](Images/image-24.png)

- port forward messaging

![alt text](Images/image-25.png)

![alt text](Images/image-26.png)

![alt text](Images/image-27.png)

## Kubernetes 
![alt text](Images/image-28.png)

![alt text](Images/image-29.png)

![alt text](Images/image-30.png)

## Destroy the deployed application
 - Apply the `aspirate destroy` to destroy and remove the resources

![alt text](Images/image-31.png)

![alt text](Images/image-32.png)

![alt text](Images/image-33.png)