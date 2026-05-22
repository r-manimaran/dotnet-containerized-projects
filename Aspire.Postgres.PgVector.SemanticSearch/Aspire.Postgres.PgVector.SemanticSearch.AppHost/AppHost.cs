var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();  

var embeddingModel = ollama.AddModel("qwen3-embedding", "qwen3-embedding:0.6b");

var postgres = builder.AddPostgres("postgres", port:6432)
    .WithImage("pgvector/pgvector", "pg17")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .AddDatabase("articles");

builder.AddProject<Projects.SemanticSearchApi>("semanticsearchapi")
       .WithReference(embeddingModel)
       .WithReference(postgres)
       .WaitFor(embeddingModel)
       .WaitFor(postgres);

builder.Build().Run();
