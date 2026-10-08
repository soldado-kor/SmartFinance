using Microsoft.EntityFrameworkCore;
using SmartFinance.Models;

namespace SmartFinance.Data;

public class AppDbContext : DbContext
{
    public DbSet<CryptoPrice> CryptoPrices { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}