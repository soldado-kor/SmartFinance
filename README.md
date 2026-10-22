# SmartFinance Crypto API

SmartFinance is a tool for collecting cryptocurrency price data, storing it in a database, and analyzing market trends.

## Features

- Fetches cryptocurrency price data from an external API and stores it in a database.
- Analyzes cryptocurrency price trends.
- Performs analysis over a user-defined time window.
- Calculates percentage price change, highest and lowest prices, and their timestamps.
- Periodically collects Bitcoin price data in the background.

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

**External Services**
- CoinGecko API

**Testing & Documentation**
- xUnit
- Swagger / OpenAPI

**Version Control**
- Git
- GitHub

## Architecture

The application follows a layered architecture with clear separation of responsibilities.

- **Controllers** handle HTTP requests, input validation and API responses.
- **Services** handle communication with the external CoinGecko API.
- **Repositories** handle database access through Entity Framework Core.
- **Analyzers** contain the business logic responsible for cryptocurrency price analysis.
- **DTOs** define the data exposed through the API.
- **BackgroundJobs** periodically collect Bitcoin price data and store it in the database.

A typical analysis request follows this flow:

~~~text
Client
  ↓
Controller
  ↓
Repository
  ↓
SQLite
  ↓
StockAnalyzer
  ↓
AnalysisResult
  ↓
AnalysisResponseDto
  ↓
HTTP Response
~~~

## API Endpoints

### Synchronize cryptocurrency price

~~~http
POST /api/crypto/sync
~~~

Fetches the current price of the selected cryptocurrency from CoinGecko and stores it in the database.

### Historical insights

~~~http
GET /api/crypto/insights/{symbol}
~~~

Analyzes all available historical data for the selected cryptocurrency.

### Time-range analysis

~~~http
GET /api/crypto/analysis/{symbol}?days=30
~~~

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


## Getting Started

### Prerequisites

- .NET 10 SDK

### Running the application

1. Clone the repository.
2. Navigate to the repository directory.
3. Build the solution:

~~~bash
dotnet build SmartFinance.sln
~~~

4. Run the application:

~~~bash
dotnet run --project SmartFinance/SmartFinance.csproj
~~~

5. Open Swagger:

~~~text
http://localhost:5150/swagger
~~~

## Example Requests

### Synchronize cryptocurrency price

~~~http
POST /api/crypto/sync
Content-Type: application/json

{
  "symbol": "BTC"
}
~~~

### Analyze all available historical data

~~~http
GET /api/crypto/insights/ETH
~~~

### Analyze a specific time window

~~~http
GET /api/crypto/analysis/BTC?days=30
~~~