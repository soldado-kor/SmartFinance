using SmartFinance.Data;
using SmartFinance.Models;

namespace SmartFinance.Repositories;

public class CryptoRepository
{
    private readonly AppDbContext _context;

    public CryptoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CryptoPrice price)
    {
        _context.CryptoPrices.Add(price);
        await _context.SaveChangesAsync();
    }

    public List<CryptoPrice> GetBySymbol(string symbol)
    {
        return _context.CryptoPrices
            .Where(x => x.Symbol == symbol)
            .OrderBy(x => x.Timestamp)
            .ToList();
    }

    public List<CryptoPrice> GetBySymbolAndDateRange(string symbol, DateTime startTime, DateTime endTime)
    {
        return _context.CryptoPrices
            .Where(x => x.Symbol == symbol && x.Timestamp >= startTime && x.Timestamp <= endTime)
            .OrderBy(x => x.Timestamp)
            .ToList();
    }
}