# WolverineRabbitMQ - User Management API

A containerized .NET 10 microservice demonstrating event-driven architecture using Wolverine framework with RabbitMQ messaging and PostgreSQL database, orchestrated with .NET Aspire.

## Architecture Overview

This project showcases a modern microservices architecture with:
- **Event-driven messaging** using Wolverine and RabbitMQ
- **Database persistence** with Entity Framework Core and PostgreSQL
- **Container orchestration** with .NET Aspire
- **API endpoints** for user management operations

## Project Structure

```
WolverineRabbitMQ/
├── UserManagementApi/          # Main API service
│   ├── Data/                   # Entity Framework models and context
│   ├── Endpoints/              # API endpoint definitions
│   ├── Features/               # Feature-based organization
│   │   └── Register/           # User registration feature
│   └── Migrations/             # Database migrations
├── WolverineRabbitMQ.AppHost/  # Aspire orchestration host
└── WolverineRabbitMQ.ServiceDefaults/ # Shared service configurations
```

## Key Features

### User Registration Flow
1. **HTTP POST** to `/users` endpoint
2. **Command handling** via Wolverine message bus
3. **Database persistence** using Entity Framework Core
4. **Event publishing** to RabbitMQ for downstream processing
5. **Event handling** for welcome email notifications (simulated)

### Technology Stack
- **.NET 10** - Latest .NET framework
- **Wolverine** - Message bus and command/query handling
- **RabbitMQ** - Message broker for event-driven communication
- **PostgreSQL** - Primary database
- **Entity Framework Core** - ORM for database operations
- **.NET Aspire** - Cloud-native orchestration
- **OpenAPI/Swagger** - API documentation

## Prerequisites

- .NET 10 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code

## Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd WolverineRabbitMQ
```

### 2. Run with Aspire
```bash
dotnet run --project WolverineRabbitMQ.AppHost
```

This will automatically:
- Start PostgreSQL container with persistent volume
- Start RabbitMQ container with management plugin
- Build and run the UserManagementApi
- Apply database migrations

### 3. Access Services
- **API**: http://localhost:5000 (or assigned port)
- **Swagger UI**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Aspire Dashboard**: http://localhost:15888

## API Usage

### Register a New User
```bash
POST /users
Content-Type: application/json

{
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "id": "01234567-89ab-cdef-0123-456789abcdef"
}
```

## Event Flow

1. **RegisterUser** command is sent to the message bus
2. **RegisterUserHandler** processes the command:
   - Creates new User entity
   - Saves to PostgreSQL database
   - Publishes **UserRegistered** event to RabbitMQ
3. **UserRegisteredHandler** processes the event:
   - Logs welcome email notification
   - Simulates email sending with delay

## Configuration

### Connection Strings
- **PostgreSQL**: Configured via Aspire service discovery
- **RabbitMQ**: Configured via Aspire service discovery

### Wolverine Configuration
```csharp
builder.Host.UseWolverine(options =>
{
    options.UseRabbitMqUsingNamedConnection("rmq")
        .AutoProvision()
        .UseConventionalRouting();
    options.Policies.DisableConventionalLocalRouting();
});
```

## Development Notes

### Database Migrations
Migrations are automatically applied in development environment. To create new migrations:

```bash
cd UserManagementApi
dotnet ef migrations add <MigrationName>
```

### Message Routing
The project uses Wolverine's conventional routing for RabbitMQ message distribution. Events are automatically routed based on message type.

### Monitoring
OpenTelemetry tracing is configured for Wolverine message handling, providing observability through the Aspire dashboard.

## Known Issues

The code review identified several areas for improvement:
- Error handling in API endpoints and database operations
- Resource disposal in service scopes
- Performance optimization in event handlers
- Code cleanup and formatting consistency

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is for educational purposes demonstrating modern .NET microservices patterns.