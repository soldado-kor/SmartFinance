using System.Net;
using SmartFinance.Tests.Infrastructure;
using SmartFinance.Data;
using SmartFinance.Models;
using SmartFinance.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace SmartFinance.Tests.Integration;

public class CryptoControllerTests
{
    [Fact]
    public async Task GetInsights_WithoutData_ReturnsNotFound()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        //Act
        var response = await client.GetAsync("/api/crypto/insights/BTC");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

     [Fact]
    public async Task GetInsights_WithOneRecord_ReturnsUnknown()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        CryptoPrice testPrice = new CryptoPrice
        {
            Symbol = "btc",
            Price = 100,
            Timestamp = DateTime.UtcNow
        };
        context.CryptoPrices.Add(testPrice);
        await context.SaveChangesAsync();

        //Act
        var response = await client.GetAsync("/api/crypto/insights/BTC");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AnalysisResponseDTO? content = await response.Content.ReadFromJsonAsync<AnalysisResponseDTO>();
        Assert.NotNull(content);
        Assert.Equal("UNKNOWN", content.Trend);
    }

         [Fact]
    public async Task GetInsights_WithRisingPrices_ReturnsUpTrend()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        CryptoPrice testPrice1 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 100,
            Timestamp = DateTime.UtcNow
        };
        CryptoPrice testPrice2 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 110,
            Timestamp = DateTime.UtcNow.AddHours(1)
        };
        context.CryptoPrices.Add(testPrice1);
        context.CryptoPrices.Add(testPrice2);
        await context.SaveChangesAsync();

        //Act
        var response = await client.GetAsync("/api/crypto/insights/BTC");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AnalysisResponseDTO? content = await response.Content.ReadFromJsonAsync<AnalysisResponseDTO>();
        Assert.NotNull(content);
        Assert.Equal("UP", content.Trend);
        Assert.Equal(10, content.ChangePercent);
        Assert.Equal(110, content.HighestPrice);
        Assert.Equal(100, content.LowestPrice);
    }

        [Fact]
    public async Task GetInsights_WithInvalidPrices_ReturnsInternalServerError()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        CryptoPrice testPrice1 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 100,
            Timestamp = DateTime.UtcNow
        };
        CryptoPrice testPrice2 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 0,
            Timestamp = DateTime.UtcNow.AddHours(1)
        };
        context.CryptoPrices.Add(testPrice1);
        context.CryptoPrices.Add(testPrice2);
        await context.SaveChangesAsync();

        //Act
        var response = await client.GetAsync("/api/crypto/insights/BTC");

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

        [Fact]
    public async Task GetAnalysis_WithoutData_ReturnsNotFound()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        //Act
        var response = await client.GetAsync("/api/crypto/analysis/btc?days=30");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

            [Fact]
    public async Task GetAnalysis_WithInvalidDays_ReturnsBadRequest()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        //Act
        var response = await client.GetAsync("/api/crypto/analysis/btc?days=0");

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

            [Fact]
    public async Task GetAnalysis_WithUnsupportedSymbol_ReturnsBadRequest()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        //Act
        var response = await client.GetAsync("/api/crypto/analysis/unsupported?days=30");

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

            [Fact]
    public async Task GetAnalysis_WithValidData_ReturnsOk()
    {
        //Arrange
        using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        CryptoPrice testPrice1 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 100,
            Timestamp = DateTime.UtcNow.AddDays(-1)
        };
        CryptoPrice testPrice2 = new CryptoPrice
        {
            Symbol = "btc",
            Price = 90,
            Timestamp = DateTime.UtcNow
        };
        context.CryptoPrices.Add(testPrice1);
        context.CryptoPrices.Add(testPrice2);
        await context.SaveChangesAsync();

        //Act
        var response = await client.GetAsync("/api/crypto/analysis/btc?days=30");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AnalysisResponseDTO? content = await response.Content.ReadFromJsonAsync<AnalysisResponseDTO>();
        Assert.NotNull(content);
        Assert.Equal("DOWN", content.Trend);
        Assert.Equal(-10, content.ChangePercent);
        Assert.Equal(100, content.HighestPrice);
        Assert.Equal(90, content.LowestPrice);
    }
    }
