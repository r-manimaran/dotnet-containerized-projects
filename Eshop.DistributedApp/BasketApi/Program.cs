var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.AddRedisDistributedCache("cache");

builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddHttpClient<CatalogApiClient>(client=>
{
    client.BaseAddress = new("https+http://catalogapi");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
}); 

builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(
                            serviceName: "keycloak",
                            realm: "eshop",
                            configureOptions: options =>
                            {
                                options.RequireHttpsMetadata = false;
                                options.Audience = "account";
                            });
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options => {
    options.SwaggerEndpoint(
    "/openapi/v1.json", "OpenAPI v1");
});

app.MapBasketEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();


