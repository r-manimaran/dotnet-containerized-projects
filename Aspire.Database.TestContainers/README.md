# Use TestContainers in CI/CD Pipleline

- Below is the github workflow actions yaml file to build the project and test the testcases.


```yaml

name: Build

on: 
    workflow_dispatch:
    push:
        branches:
            - main
        paths:
            - 'Aspire.Database.TestContainers'
env:
    DOTNET_VERSION: "9.x"

jobs:
    build:
        runs-on: ubuntu-latest
        
        steps: 
            - name: Checkout to the branch
              uses: actions/checkout@v3

            - name: Setup .Net
              uses: actions/setup-dotnet@v3
              with:
                dotnet-version: ${{env.DOTNET_VERSION}}
            
            - name: Restore
              working-directory: Aspire.Database.TestContainers
              run: dotnet restore ./Aspire.Database.TestContainers.sln

            - name: Build
              working-directory: Aspire.Database.TestContainers
              run: dotnet build ./Aspire.Database.TestContainers.sln --configuration Release --no-restore
            
            - name: Test
              working-directory: Aspire.Database.TestContainers
              run: dotnet test ./Aspire.Database.TestContainers.sln --configuration Release --no-restore --no-build

```

## output

![alt text](image.png)

## RabbitMQ Endpoint to publish a message

![alt text](image-1.png)

**Consume Message**:

![alt text](image-2.png)