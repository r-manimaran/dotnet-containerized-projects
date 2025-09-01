using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

var storage = builder.AddAzureStorage("storage").RunAsEmulator()
    .ConfigureInfrastructure((infrastructure) =>
    {
        var storageAccount = infrastructure.GetProvisionableResources().OfType<StorageAccount>().FirstOrDefault(r=>r.BicepIdentifier=="storage")
        ?? throw new InvalidOperationException($"Could not find configured storage account with name 'storage'");

        storageAccount.AllowBlobPublicAccess = false;
    });
var blobs = storage.AddBlobs("blobs");

var queues = storage.AddQueue("queues");


var azfun = builder.AddAzureFunctionsProject<Projects.AzFuncThumbNailGenerator>("azfuncthumbnailgenerator")
     .WithReference(queues)
        .WithReference(blobs)
        .WaitFor(storage)
        .WithRoleAssignments(storage,
        StorageBuiltInRole.StorageAccountContributor, StorageBuiltInRole.StorageBlobDataOwner, // Storage Account Access role for the function
        StorageBuiltInRole.StorageQueueDataContributor // Storage Queues Access role
        ).WithHostStorage(storage);


builder.AddProject<Projects.Aspire_ThumbNail_WebApp>("aspire-thumbnail-webapp")
        .WithExternalHttpEndpoints()
        .WithReference(queues)
        .WithReference(blobs)
        .WaitFor(azfun);

builder.Build().Run();
