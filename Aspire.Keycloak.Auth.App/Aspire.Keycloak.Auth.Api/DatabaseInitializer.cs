
using Dapper;
using Npgsql;

namespace Aspire.Keycloak.Auth.Api
{
    public class DatabaseInitializer : IHostedService
    {
        private readonly NpgsqlConnection _connection;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(NpgsqlConnection connection, ILogger<DatabaseInitializer> logger)
        {
            _connection = connection;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
           await CreateStocksTableAsync();

            await InsertMockDataAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public async Task CreateStocksTableAsync()
        {
            var tableCreationQuery = @"
                CREATE TABLE IF NOT EXISTS stocks (
                  id SERIAL PRIMARY KEY,
                  symbol VARCHAR(50) NOT NULL,
                  name VARCHAR(50) NOT NULL,
                  price DECIMAL(18,2) NOT NULL,
                 quantity INT NOT NULL
                );
                ";
            await _connection.ExecuteAsync(tableCreationQuery);
        }

        public async Task InsertMockDataAsync()
        {
            var existingStocks = await _connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM stocks;");
            if(existingStocks > 0)
            {
                return;
            }

            var mockStocks = new List<object>()
            {
                new {Symbol = "AAPL", Name="Apple Inc.", Price=175.25m, Quantity=400 },
                new {Symbol = "GOOG", Name="Alphabet Inc.", Price=270.25m, Quantity=200 },
                new {Symbol = "AMZN", Name="Amazon.Com", Price=3341.00m, Quantity=500 },
                new {Symbol = "MSFT", Name="Microsoft Corporation", Price=295.50m, Quantity=400 },
                new {Symbol = "TSLA", Name="Tesla Inc.", Price=175.25m, Quantity=300 }
            };

            var insertQuery = @"
                                INSERT INTO stocks (symbol,name,price,quantity)
                                VALUES (@symbol, @Name, @Price, @Quantity);
                               ";
            await _connection.ExecuteAsync(insertQuery, mockStocks);
        }
    }
}
