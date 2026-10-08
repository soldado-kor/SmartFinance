namespace SmartFinance.Exceptions;

public class UnsupportedSymbolException : Exception
{
    public UnsupportedSymbolException(string symbol)
        : base($"Unsupported symbol: {symbol}")
        {
        }
}