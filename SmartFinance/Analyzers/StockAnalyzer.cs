using SmartFinance.Models;
using SmartFinance.Exceptions;
using SmartFinance.DTOs;

namespace SmartFinance.Analyzers;


public class StockAnalyzer
{
    public AnalysisResult Analyze(List<CryptoPrice> prices)
    {
        if (prices == null || prices.Count < 2)
        {
            return new AnalysisResult { Trend = "UNKNOWN" };
        }

        if(prices.Any(x => x.Price <= 0))
        {
            throw new InvalidCryptoPriceException();
        }

        var first = prices.First();
        var last = prices.Last();

        var firstPrice = first.Price;
        var lastPrice = last.Price;

        var highest = prices.MaxBy(x => x.Price)!;
        var lowest = prices.MinBy(x => x.Price)!;

        var changePercent = ((lastPrice - firstPrice) / firstPrice) * 100;

        var trend = changePercent switch
        {
            > 1 => "UP",
            < -1 => "DOWN",
            _ => "STABLE"
        };

        return new AnalysisResult
        {
            Trend = trend,
            ChangePercent = changePercent,
            HighestPrice = highest.Price,
            HighestPriceTimestamp = highest.Timestamp,
            LowestPrice = lowest.Price,
            LowestPriceTimestamp = lowest.Timestamp
        };
    }
}