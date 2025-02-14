using CustomerApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace CustomerApi.Test.Integration;

public class ApiFactory : WebApplicationFactory<IApiAssemblyMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;
    public ApiFactory()
    {
        _msSqlContainer = new MsSqlBuilder()
                            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                            .WithPassword("Your_password123$")
                            .WithEnvironment("ACCEPT_EULA", "Y")                                     
                            .WithCleanUp(true)                    
                            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(
            services =>
            {
                services.Remove<AppDbContext>();

                services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(_msSqlContainer.GetConnectionString()));
            }
            );


        base.ConfigureWebHost(builder);
    }

    public Task InitializeAsync()
    {
        return _msSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }
}
