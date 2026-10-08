using SmartFinance.Models;
using SmartFinance.Analyzers;
using Xunit;

namespace SmartFinance.Tests;

public class StockAnalyzerTests
{
    [Fact]
    public void AnalyzeUPtrend()
    {
        var cryptoPrice1 = new CryptoPrice
    {  
        Price = 100,
        Timestamp = new DateTime(2026, 1, 12)
    };
            var cryptoPrice2 = new CryptoPrice
    {  
        Price = 120,
        Timestamp = new DateTime(2026, 1, 13)
    };

    var prices = new List<CryptoPrice>();

    prices.Add(cryptoPrice1);
    prices.Add(cryptoPrice2);

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal(20, result.ChangePercent);
    Assert.Equal("UP", result.Trend);
    }

        [Fact]
    public void AnalyzeDOWNtrend()
    {
        var cryptoPrice1 = new CryptoPrice
    {  
        Price = 120,
        Timestamp = new DateTime(2026, 1, 12)
    };
            var cryptoPrice2 = new CryptoPrice
    {  
        Price = 100,
        Timestamp = new DateTime(2026, 1, 13)
    };

    var prices = new List<CryptoPrice>();

    prices.Add(cryptoPrice1);
    prices.Add(cryptoPrice2);

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal(-16.67m, result.ChangePercent, 2);
    Assert.Equal("DOWN", result.Trend);
    }

        [Fact]
    public void AnalyzeSTABLEtrend()
    {
        var cryptoPrice1 = new CryptoPrice
    {  
        Price = 100,
        Timestamp = new DateTime(2026, 1, 12)
    };
            var cryptoPrice2 = new CryptoPrice
    {  
        Price = 100.5m,
        Timestamp = new DateTime(2026, 1, 13)
    };

    var prices = new List<CryptoPrice>();

    prices.Add(cryptoPrice1);
    prices.Add(cryptoPrice2);

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal(0.5m, result.ChangePercent, 2);
    Assert.Equal("STABLE", result.Trend);
    }

        [Fact]
    public void Analyze1PRICEtrend()
    {
        var cryptoPrice1 = new CryptoPrice
    {  
        Price = 100,
        Timestamp = new DateTime(2026, 1, 12)
    };


    var prices = new List<CryptoPrice>();

    prices.Add(cryptoPrice1);

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal("UNKNOWN", result.Trend);
    }

        [Fact]
    public void AnalyzeNULL_PRICE()
    {
    List<CryptoPrice>? prices = null;

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal("UNKNOWN", result.Trend);
    }
}