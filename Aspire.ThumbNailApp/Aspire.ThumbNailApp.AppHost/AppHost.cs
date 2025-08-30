var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Aspire_ThumbNail_WebApp>("aspire-thumbnail-webapp");

builder.AddAzureFunctionsProject<Projects.AzFuncThumbNailGenerator>("azfuncthumbnailgenerator");

builder.Build().Run();
