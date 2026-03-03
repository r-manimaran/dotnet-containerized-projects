var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume();
   // .WithGPUSupport();

var voyage4 = ollama.AddHuggingFaceModel("vouage-4-nano", "jsonMartin/voyage-4-nano-gguf");

var mongo = builder.AddConnectionString("mongo");

builder.AddProject<Projects.RecommendationsApi>("recommendationsapi")
   .WithReference(voyage4)
   .WithReference(mongo)
   .WaitFor(voyage4)
   .WaitFor(mongo);

builder.Build().Run();
