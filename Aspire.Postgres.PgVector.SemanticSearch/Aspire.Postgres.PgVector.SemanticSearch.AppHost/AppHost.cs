var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();  

var embeddingModel = ollama.AddModel("qwen3-embedding", "qwen3-embedding:0.6b");

var postgres = builder.AddPostgres("postgres", port: 6432)
    .WithImage("pgvector/pgvector", "pg17")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();
    //.AddDatabase("articles");

var articlesDb = postgres.AddDatabase("articles");

var articlesCoreDb = postgres.AddDatabase("articles-core");

builder.AddProject<Projects.SemanticSearchDapperApi>("semanticsearchapi")
       .WithReference(embeddingModel)
       .WithReference(articlesDb)
       .WaitFor(embeddingModel)
       .WaitFor(articlesDb);

builder.AddProject<Projects.SemanticSearchEFCore>("semanticsearchefcore")
        .WithReference(embeddingModel)
        .WithReference(articlesCoreDb)
        .WaitFor(embeddingModel)
        .WaitFor(articlesCoreDb);

builder.Build().Run();
