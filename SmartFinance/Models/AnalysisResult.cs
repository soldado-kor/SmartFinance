namespace SmartFinance.Models;
public class AnalysisResult
{
    public string Trend { get; set; } = "UNKNOWN";
    public decimal ChangePercent { get; set; }

    public decimal HighestPrice { get; set; }
    public DateTime HighestPriceTimestamp { get; set; }

    public decimal LowestPrice { get; set; }
    public DateTime LowestPriceTimestamp { get; set; }
}