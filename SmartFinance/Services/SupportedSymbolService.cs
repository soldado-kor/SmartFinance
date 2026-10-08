namespace SmartFinance.Services;

public class SupportedSymbolService
{
    HashSet<string> _supportedSymbols = new HashSet<string>
    {
        "btc", "eth"
    };

    public bool IsSupported(string symbol)
    {
        return _supportedSymbols.Contains(symbol);
    }
}