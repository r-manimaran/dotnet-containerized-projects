## KubeCtl Commands

kubectl - Command-line tool for interacting with Kubernetes Clusters.

**Cluster Information**
```bash
# View Cluster information
> kubectl cluster-info

# check kubectl version
> kubectl version 

# View nodes in the Cluser
> kubectl get nodeskb
```

```bash
# Create or updates the resources using the deployment.yml file
> kubectl apply -f deployment.yml

# Apply from multiple files
> kubectl apply -f deployment.yml -f service.yml

# Apply from a directory
> kubectl apply -f ./kubernetes-configs/

# Apply from a URL
> kubectl apply -f https://raw.githubusercontent.com/example/manifest.yml

# View all resources
> kubectl get all

# View Depployments
> kubectl get deployments

# List all pods
> kubectl get pods

# Watch pads in real-time
> kubectl get pods -w

# List all pods
> kubectl get pods

# List pods with labels
> kubectl get pods --show-labels

# Get all Nodes
> kubectl get nodes

# Get all resources for all namespace
> kubectl get all -A

# Describe commands
> kubectl describe node <nodename>
> kubectl describe pod <podname>


# Get detailed information about pods
> kubectl get pods -o wide

# View Services
> kubectl get services

# Describe specific deployment
> kubectl describe deployment blg-api-deployment

# Check pod logs
> kubectl logs <pod-name>

# Delete resources using the same YAML file
> kubectl delete -f deployment.yml

# Delete specific resources by type and name
> kubectl delete deployment blog-api-deployment
> kubectl delete service blog-api-service

# Delete all pods
> kubectl delete pods --all

# Delete all deployments
> kubectl delete deployments -all

# Delete all services
> kubectl delete services -all

# Delete all in one command
> kubectl delete all -all

# Delete all resources created from the multiple yaml files
> kubectl delete -f .
