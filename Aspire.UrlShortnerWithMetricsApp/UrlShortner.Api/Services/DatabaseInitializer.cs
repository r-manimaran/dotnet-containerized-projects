using Dapper;
using Npgsql;

namespace UrlShortner.Api.Services;

public class DatabaseInitializer : BackgroundService
{
    private readonly NpgsqlDataSource _datasource;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(NpgsqlDataSource datasource, 
                               IConfiguration configuration,
                               ILogger<DatabaseInitializer> logger)
    {
        _datasource = datasource;
        _configuration = configuration;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await CreateDatabaseIfNotExists(stoppingToken);
            await InitializeSchema(stoppingToken);

            _logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing database");
            throw;
        }
    }

    private async Task CreateDatabaseIfNotExists(CancellationToken stoppingToken)
    {
        var connectionString = _configuration.GetConnectionString("url-shortner");
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        string? databaseName = builder.Database;
        builder.Database = "postgres";

        await using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync(stoppingToken);

        bool databaseExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname=@databaseName)",
                new { databaseName });
        if (!databaseExists)
        {
            _logger.LogInformation("Database not exists creating the DB :{databaseName}",databaseName);
            await connection.ExecuteAsync($"CREATE DATABASE \"{databaseName}\"");
        }
        
    }

    private async Task InitializeSchema(CancellationToken stoppingToken)
    {
        const string createTableSql =
            """
            CREATE TABLE IF NOT EXISTS shortened_urls (
                id SERIAL PRIMARY KEY,
                short_code VARCHAR(10) UNIQUE NOT NULL,
                original_url TEXT NOT NULL,
                created_on TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
                );

            CREATE INDEX IF NOT EXISTS idx_short_code ON shortened_urls(short_code);

            CREATE TABLE IF NOT EXISTS url_visits(
                id SERIAL PRIMARY KEY,
                short_code VARCHAR(10) NOT NULL,
                visited_on TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                user_agent TEXT,
                referer TEXT,
                FOREIGN KEY (short_code) REFERENCES shortened_urls(short_code)
                );

            CREATE INDEX IF NOT EXISTS idx_visits_short_code on url_visits(short_code);
            """;

        await using var command = _datasource.CreateCommand(createTableSql);
        await command.ExecuteNonQueryAsync(stoppingToken);
    }
}
