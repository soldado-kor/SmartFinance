using SmartFinance.Models;
using SmartFinance.Analyzers;
using SmartFinance.Exceptions;
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

         [Fact]
    public void AnalyzeEmptyPrice_ReturnsUnknown()
    {
    var prices = new List<CryptoPrice>();

    var analyzer = new StockAnalyzer();

    var result = analyzer.Analyze(prices);

    Assert.Equal("UNKNOWN", result.Trend);
    }

         [Fact]
     public void AnalyzeZeroPrice_ReturnsError()
    {
        var cryptoPrice1 = new CryptoPrice
    {
        Price = 0,
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

    Assert.Throws<InvalidCryptoPriceException>(
        () => analyzer.Analyze(prices)
        );
    }
         [Fact]
     public void AnalyzeNegativePrice_ReturnsError()
    {
        var cryptoPrice1 = new CryptoPrice
    {
        Price = -10,
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

    Assert.Throws<InvalidCryptoPriceException>(
        () => analyzer.Analyze(prices)
        );
    }
}