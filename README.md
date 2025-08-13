# Order Microservice

A learning project to practice **Domain-Driven Design (DDD)** and modern **.NET 9** microservice patterns.  
It includes API versioning, validation, messaging, persistence, health checks, observability, and container/Kubernetes deployment.

---

## Tech Stack

- **Runtime**: .NET 9 (`net9.0`)
- **Architecture**: DDD + CQRS (MediatR), clean boundaries (Domain, Application, Infrastructure, API)
- **API**: ASP.NET Core, `Asp.Versioning` (REST versioning), Swagger/Scalar
- **Validation**: FluentValidation, Ardalis.GuardClauses
- **Persistence**: EF Core (SQL Server/PostgreSQL), MongoDB, EventStoreDB
- **Messaging**: MassTransit + RabbitMQ
- **Caching**: EasyCaching (InMemory/Redis)
- **Security**: Duende IdentityServer (JWT)
- **Health**: AspNetCore.HealthChecks (+ UI)
- **Observability**: Serilog, OpenTelemetry (traces/metrics/logs), Grafana/Zipkin/OTLP exporters
- **Testing**: xUnit + FluentAssertions + NSubstitute, Testcontainers for integration tests
- **Gateway/Reverse Proxy**: YARP
- **Containers/Orchestration**: Docker, Kubernetes (Helm/manifest), K8s health/readiness probes

---

## Solution Layout (DDD-friendly)

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- (Optional) Kubernetes: **minikube** or **kind**, `kubectl`, and optionally **Helm**
- A running instance of any dependencies you enable (e.g., PostgreSQL, RabbitMQ)

### App configuration
Use `appsettings.json` + environment variables. Common envs:

| Variable | Description | Example |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Environment | `Development` |
| `ConnectionStrings__Default` | SQL connection | `Host=localhost;Port=5432;Database=orders;Username=postgres;Password=postgres` |
| `RabbitMq__Host` | RabbitMQ URI | `amqp://guest:guest@localhost:5672` |
| `Mongo__ConnectionString` | MongoDB | `mongodb://localhost:27017` |

------

## Running Locally (without Docker)

```bash
dotnet restore
dotnet build -c Debug
# replace with your API project path
dotnet run --project src/OrderService.Api
