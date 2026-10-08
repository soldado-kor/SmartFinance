namespace SmartFinance.Exceptions;

public class NoApiDataException : Exception
{
    public NoApiDataException()
        : base("No API data available.")
        {
        }
}