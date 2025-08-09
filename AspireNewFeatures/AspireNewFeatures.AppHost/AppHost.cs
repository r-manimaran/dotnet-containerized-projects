var builder = DistributedApplication.CreateBuilder(args);

var webAddressReference = builder.AddExternalService("BlogPostUrl", "https://jsonplaceholder.typicode.com");

var githubModel = builder.AddGitHubModel("ai-model", "openai/gpt4o-mini");


builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(webAddressReference)
    .WithReference(githubModel);

builder.Build().Run();
