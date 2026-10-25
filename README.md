# SmartFinance Crypto API

SmartFinance is a tool for collecting cryptocurrency price data, storing it in a database, and analyzing market trends.

## Features

- Fetches cryptocurrency price data from an external API and stores it in a database.
- Analyzes cryptocurrency price trends.
- Performs analysis over a user-defined time window.
- Calculates percentage price change, highest and lowest prices, and their timestamps.
- Periodically collects Bitcoin price data in the background.
- Supports containerized deployment using Docker and Docker Compose.
- Persists SQLite database data across container recreations using a named Docker volume.
- Automated build and unit test execution through GitHub Actions on pushes to `main` and pull requests targeting `main`.

## Tech Stack

**Backend**
- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core

**Database**
- SQLite

**Architecture & API**
- REST API
- Dependency Injection
- Repository Pattern
- BackgroundService

**Containerization & Deployment**
- Docker
- Docker Compose

**External Services**
- CoinGecko API

**Testing & Documentation**
- xUnit
- ASP.NET Core WebApplicationFactory
- Swagger / OpenAPI

**Version Control**
- Git
- GitHub

**CI/CD**
- GitHub Actions (continuous integration)

## Architecture

The application follows a layered architecture with clear separation of responsibilities.

- **Controllers** handle HTTP requests, input validation and API responses.
- **Services** handle communication with the external CoinGecko API.
- **Repositories** handle database access through Entity Framework Core.
- **Analyzers** contain the business logic responsible for cryptocurrency price analysis.
- **DTOs** define the data exposed through the API.
- **BackgroundJobs** periodically collect Bitcoin price data and store it in the database.

The following diagram illustrates the request flow for the historical insights endpoint:

```text
Client
  ↓
CryptoController (Insights)
  ↓
Repository ↔ SQLite
  ↓
StockAnalyzer
  ↓
AnalysisResult
  ↓
AnalysisResponseDTO
  ↓
HTTP 200 OK
```

### Containerization

The application is containerized using a multi-stage Docker build. The .NET SDK image is used to restore dependencies and publish the application, while the final image uses the ASP.NET Core runtime image.

Docker Compose defines the application service, port mapping, environment variables and a named volume for persistent SQLite database storage.

## API Endpoints

### Synchronize cryptocurrency price

```http
POST /api/crypto/sync
```

Fetches the current price of the selected cryptocurrency from CoinGecko and stores it in the database.

### Historical insights

```http
GET /api/crypto/insights/{symbol}
```

Analyzes all available historical data for the selected cryptocurrency.

### Time-range analysis

```http
GET /api/crypto/analysis/{symbol}?days=30
```

Analyzes cryptocurrency price data within a user-defined time window.

The `days` parameter must be between 1 and 365.

## Background Processing

The application uses ASP.NET Core `BackgroundService` to periodically collect cryptocurrency price data.

Currently, the background service:

- runs once per minute,
- retrieves Bitcoin price data from CoinGecko,
- stores the price and timestamp in SQLite.

The current implementation focuses on Bitcoin and can be extended to support additional cryptocurrencies in the future.

## Testing

The project includes unit and integration tests using xUnit.

### Unit tests

Unit tests cover the `StockAnalyzer` business logic and the following scenarios:

- UP, DOWN and STABLE trends
- Insufficient or empty input data
- Null input
- Zero or negative cryptocurrency prices

### Price validation

The `StockAnalyzer` throws an `InvalidCryptoPriceException` when any price is less than or equal to zero.

The API controller catches this exception and returns an HTTP 500 response.

### Integration tests

Integration tests cover the `CryptoController` API endpoints using `WebApplicationFactory`.

The tests verify:

- HTTP 200 responses and returned analysis data for valid requests
- HTTP 404 responses when no price data is available
- HTTP 400 responses for invalid parameters and unsupported symbols
- HTTP 500 responses when invalid price data causes an analysis error
- Trend calculations, percentage changes, and highest and lowest prices

Each test factory instance uses an isolated temporary SQLite database. SQLite connection pooling is disabled in the test configuration to allow reliable cleanup of temporary database files.

### Running tests

Run all tests from the solution directory:

```bash
dotnet test
```

Build the project:

```bash
dotnet build
```

The current test suite contains 16 tests, all passing.

## Continuous Integration

The project uses GitHub Actions to automatically validate changes through a CI workflow.

The workflow is triggered by:
- Pushes to the `main` branch.
- Pull requests targeting the `main` branch.

The workflow runs on a GitHub-hosted Ubuntu runner with the .NET 10 SDK and performs the following steps:

1. Restores .NET dependencies.
2. Builds the solution.
3. Runs the unit and integration test suite.
4. Builds the Docker image and starts the application using Docker Compose.
5. Performs an API smoke test by checking the Swagger/OpenAPI endpoint at `http://localhost:5150/swagger/v1/swagger.json`.
6. Collects Docker Compose logs if the application startup or readiness check fails.
7. Stops the Compose services and removes the associated named volume during cleanup.

A failed build, test, or API smoke test causes the workflow to fail. This helps prevent changes that do not pass the configured checks from being merged when the repository's branch protection rules require the CI status check to pass.

Build and test results are available in the repository's **Actions** tab and in pull request checks.

## Getting Started

### Running locally

#### Prerequisites

- .NET 10 SDK

#### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/soldado-kor/SmartFinance.git
   ```

2. Navigate to the repository directory:

   ```bash
   cd SmartFinance
   ```

3. Build the solution:

   ```bash
   dotnet build SmartFinance.sln
   ```

4. Run the application:

   ```bash
   dotnet run --project SmartFinance/SmartFinance.csproj
   ```

5. Open Swagger UI:

   ```text
   http://localhost:5150/swagger
   ```

### Running with Docker

#### Prerequisites

- Docker Desktop with Docker Compose support

#### Start the application

Run the following command from the repository root:

```bash
docker compose up --build -d
```

This builds the Docker image and starts the API container in the background.

Open Swagger UI:

```text
http://localhost:5150/swagger
```

#### Stop the application

Stop and remove the Compose containers and network:

```bash
docker compose down
```

The named Docker volume containing the SQLite database is preserved when running `docker compose down`. This allows the database to persist when the container is recreated.

To remove the database volume as well, use:

```bash
docker compose down -v
```

**Warning:** This permanently deletes the database data stored in the Compose-managed volume.

#### Database configuration

The application uses `ConnectionStrings:DefaultConnection` from `appsettings.json` by default. Docker Compose overrides this setting to use `/data/app.db`, stored in the named Docker volume.

Local .NET execution continues to use `app.db` in the application's working directory.

## Example Requests

### Synchronize cryptocurrency price

```http
POST /api/crypto/sync
Content-Type: application/json

{
  "symbol": "BTC"
}
```

### Analyze all available historical data

```http
GET /api/crypto/insights/ETH
```

> Note: Ensure that sufficient ETH price records have been collected using the sync endpoint before requesting historical insights.

### Analyze a specific time window

```http
GET /api/crypto/analysis/BTC?days=30
```