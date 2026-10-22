using Microsoft.Extensions.Hosting;
using SmartFinance.BackgroundJobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartFinance.Data;

namespace SmartFinance.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    string TemporaryDatabaseDirectory =>
     Path.Combine(Path.GetTempPath(), _databaseName);
     string TemporaryDatabasePath =>
      Path.Combine(TemporaryDatabaseDirectory, $"{_databaseName}.db");
    
    public CustomWebApplicationFactory()
    {
        Directory.CreateDirectory(TemporaryDatabaseDirectory);
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IHostedService>();


            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={TemporaryDatabasePath};Pooling=False"));
        });
    }

    public async Task InitializeDatabaseAsync()
{
    using var scope = Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.EnsureCreatedAsync();
}

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (Directory.Exists(TemporaryDatabaseDirectory))
        {
            Directory.Delete(TemporaryDatabaseDirectory, true);
        }
    }
}
