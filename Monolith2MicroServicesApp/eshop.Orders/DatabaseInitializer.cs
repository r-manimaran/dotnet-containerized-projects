using Npgsql;
using Dapper;
namespace eshop.Orders;

public class DatabaseInitializer
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(NpgsqlDataSource dataSource,
                               IConfiguration configuration,
                               ILogger<DatabaseInitializer> logger)
    {
        _dataSource = dataSource;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database initialization");
            await EnsureDatabaseExists();
            await InitializeDatabase();
            _logger.LogInformation("Initialized database successfully");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occured while initializing the database");
        }
    }

    private async Task EnsureDatabaseExists()
    {
        string connectionString = _configuration.GetConnectionString("Database")!;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        string? databaseName = builder.Database;
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync();

        bool databaseExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(select 1 FROM pg_database WHERE datname=@databaseName)",
            new {databaseName});
        if(!databaseExists)
        {
            _logger.LogInformation("Creating database {DatabaseName}", databaseName);
            await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
        }
    }

    private async Task InitializeDatabase()
    {

        const string sql =
            """
            --create schema if it does not exists
            CREATE SCHEMA IF NOT EXISTS orders;
    

            -- Create orders table if it is not exists
            CREATE TABLE IF NOT EXISTS orders.orders(
                id UUID PRIMARY KEY,
                customer_name VARCHAR(255) NOT NULL,
                shipping_address VARCHAR(255) NOT NULL,
                product_name VARCHAR(255) NOT NULL,
                quantity INTEGER NOT NULL,
                total_price DECIMAL(18,2) NOT NULL,
                order_date TIMESTAMP WITH TIME ZONE NOT NULL
                );

            """;
        using var connection = await _dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql);
    }
   
}
