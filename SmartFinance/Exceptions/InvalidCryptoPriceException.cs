namespace SmartFinance.Exceptions;

public class InvalidCryptoPriceException : Exception
{
    public InvalidCryptoPriceException()
        : base("Price data is invalid.")
        {
        }
}