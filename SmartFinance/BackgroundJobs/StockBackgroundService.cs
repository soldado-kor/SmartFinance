using SmartFinance.Repositories;
using SmartFinance.Models;
using SmartFinance.Services;

namespace SmartFinance.BackgroundJobs;

public class StockBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;

    public StockBackgroundService(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<FmpService>();
            var repo = scope.ServiceProvider.GetRequiredService<CryptoRepository>();

            var price = await service.GetStockPrice("btc");

            await repo.AddAsync(new CryptoPrice
            {
                Symbol = "btc",
                Price = price,
                Timestamp = DateTime.UtcNow
            });

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}