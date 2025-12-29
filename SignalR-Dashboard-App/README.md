# SignalR Stock Dashboard App

A real-time stock price dashboard application built with ASP.NET Core SignalR, PostgreSQL, and vanilla JavaScript. The application provides live stock price updates using WebSocket connections and displays interactive charts for multiple stock tickers.

## 🏗️ Architecture

### Backend (.NET 9)
- **ASP.NET Core Web API** with SignalR Hub
- **PostgreSQL** database for stock price persistence
- **Dapper** for database operations
- **OpenTelemetry** for observability and tracing
- **Docker** containerization support

### Frontend
- **Vanilla JavaScript** with SignalR client
- **Chart.js** for real-time price charts
- **Tailwind CSS** for responsive UI
- **Dark/Light mode** toggle

### Infrastructure
- **Docker Compose** for multi-container orchestration
- **PostgreSQL 17** database container
- **Aspire Dashboard** for telemetry visualization

## 🚀 Features

- **Real-time Stock Updates**: Live price updates via SignalR WebSocket connections
- **Interactive Charts**: Mini charts showing price history for each stock
- **Responsive Design**: Mobile-friendly interface with dark/light mode
- **Stock Management**: Add/remove stocks dynamically
- **Data Persistence**: Stock prices stored in PostgreSQL database
- **Observability**: OpenTelemetry integration with Aspire Dashboard
- **Containerized**: Full Docker support for easy deployment

## 📁 Project Structure

```
SignalR-Dashboard-App/
├── StocksApi/                          # ASP.NET Core Web API
│   ├── Endpoints/                      # API endpoints
│   ├── Realtime/                       # SignalR hub and related services
│   │   ├── StocksFeedHub.cs           # SignalR hub for real-time updates
│   │   ├── StocksFeedUpdater.cs       # Background service for price updates
│   │   └── ActiveTickerManager.cs     # Manages active stock subscriptions
│   ├── Stocks/                         # Stock-related services and models
│   │   ├── StockService.cs            # Core stock price service
│   │   └── StocksClient.cs            # External API client
│   ├── Program.cs                      # Application entry point
│   ├── Dockerfile                      # Docker configuration
│   └── appsettings.json               # Application configuration
├── Front-end/                          # Client-side application
│   ├── index.html                      # Main HTML page
│   └── script.js                       # JavaScript with SignalR client
├── docker-compose.yml                  # Multi-container orchestration
└── README.md                          # Project documentation
```

## 🛠️ Prerequisites

- [Docker](https://www.docker.com/get-started) and Docker Compose
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local development)
- [Python 3.x](https://www.python.org/downloads/) (for serving frontend locally)

## 🚀 Quick Start

### Option 1: Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SignalR-Dashboard-App
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

3. **Access the applications**
   - **Stock API**: http://localhost:5001
   - **Swagger UI**: http://localhost:5001/swagger
   - **Aspire Dashboard**: http://localhost:18888
   - **PostgreSQL**: localhost:5432

4. **Serve the frontend**
   ```bash
   cd Front-end
   python -m http.server 3000
   ```

5. **Open the dashboard**
   - Navigate to http://localhost:3000

### Option 2: Local Development

1. **Start PostgreSQL**
   ```bash
   docker run -d --name postgres-stocks \
     -e POSTGRES_USER=postgres \
     -e POSTGRES_PASSWORD=postgres \
     -p 5432:5432 \
     postgres:17
   ```

2. **Run the API**
   ```bash
   cd StocksApi
   dotnet run
   ```

3. **Serve the frontend**
   ```bash
   cd Front-end
   python -m http.server 3000
   ```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__Database` | PostgreSQL connection string | `Host=postgres;Port=5432;Database=stocks;Username=postgres;Password=postgres` |
| `Cors__AllowedOrigin` | Frontend origin for CORS | `http://localhost:3000` |
| `StockUpdateOptions__UpdateInterval` | Price update frequency | `00:00:01` (1 second) |
| `StockUpdateOptions__MaxPercentageChange` | Max price change per update | `0.02` (2%) |

### API Configuration

The application uses simulated stock data by default. To integrate with real stock APIs:

1. Update `appsettings.json`:
   ```json
   {
     "Stocks": {
       "ApiUrl": "https://api.example.com",
       "ApiKey": "your-api-key"
     }
   }
   ```

2. Implement the external API client in `StocksClient.cs`

## 📊 Default Stocks

The dashboard loads with these pre-configured stocks:
- **AMZN** (Amazon)
- **MSFT** (Microsoft)
- **META** (Meta Platforms)
- **NVDA** (NVIDIA)
- **TSLA** (Tesla)
- **BABA** (Alibaba)
- **PYPL** (PayPal)

## 🔌 API Endpoints

### REST API
- `GET /api/stocks/{ticker}` - Get latest stock price
- `GET /openapi/v1.json` - OpenAPI specification

### SignalR Hub
- **Hub URL**: `/stocks-feed`
- **Methods**:
  - `JoinStockGroup(ticker)` - Subscribe to stock updates
  - `LeaveStockGroup(ticker)` - Unsubscribe from stock updates
- **Events**:
  - `ReceiveStockPriceUpdate(stockUpdate)` - Receive real-time price updates

## 🗄️ Database Schema

```sql
CREATE TABLE stock_prices (
    id SERIAL PRIMARY KEY,
    ticker VARCHAR(10) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_stock_prices_ticker_timestamp 
ON stock_prices(ticker, timestamp DESC);
```

## 📈 Monitoring & Observability

The application includes comprehensive observability features:

- **OpenTelemetry Tracing**: Distributed tracing across all components
- **Aspire Dashboard**: Real-time telemetry visualization at http://localhost:18888
- **Structured Logging**: JSON-formatted logs with correlation IDs
- **Health Checks**: Built-in health monitoring endpoints

## 🎨 Frontend Features

### Real-time Updates
- WebSocket connection via SignalR
- Automatic reconnection on connection loss
- Live price and percentage change indicators

### Interactive UI
- **Add Stocks**: Enter any ticker symbol to add to dashboard
- **Remove Stocks**: Click the X button to remove stocks
- **Dark Mode**: Toggle between light and dark themes
- **Responsive Design**: Works on desktop, tablet, and mobile

### Charts
- Mini line charts for each stock
- 30-point price history
- Auto-scaling Y-axis
- Color-coded price changes (green/red)

## 🐳 Docker Services

| Service | Image | Port | Description |
|---------|-------|------|-------------|
| `stocksapi` | Custom (.NET 9) | 5001 | Main API and SignalR hub |
| `postgres` | postgres:17 | 5432 | PostgreSQL database |
| `aspire-dashboard` | mcr.microsoft.com/dotnet/aspire-dashboard:9.0 | 18888 | Telemetry dashboard |

## 🔧 Development

### Building the API
```bash
cd StocksApi
dotnet build
dotnet test  # If tests are available
```

### Database Migrations
The application automatically initializes the database schema on startup via `DatabaseInitializer.cs`.

### Adding New Features
1. **New Stock Data Sources**: Implement `IStockDataProvider` interface
2. **Additional Endpoints**: Add to `StocksEndpoints.cs`
3. **Frontend Components**: Extend `StockWidget` class in `script.js`

## 🚨 Troubleshooting

### Common Issues

1. **CORS Errors**
   - Ensure frontend is served from `http://localhost:3000`
   - Check `Cors:AllowedOrigin` in `appsettings.json`

2. **SignalR Connection Failed**
   - Verify API is running on port 5001
   - Check browser console for WebSocket errors
   - Ensure HTTPS certificates are trusted

3. **Database Connection Issues**
   - Verify PostgreSQL container is running
   - Check connection string in configuration
   - Ensure database is initialized

### Logs and Debugging
- **API Logs**: Check Docker logs with `docker-compose logs stocksapi`
- **Database Logs**: Check with `docker-compose logs postgres`
- **Browser Console**: Check for JavaScript errors and SignalR connection status

## 📝 License

This project is for educational purposes. Please ensure you comply with any stock data provider terms of service when using real market data.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

---

**Note**: This application uses simulated stock data for demonstration purposes. For production use with real market data, integrate with a licensed stock data provider and ensure compliance with their terms of service.