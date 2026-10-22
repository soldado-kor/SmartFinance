using SmartFinance.Services;
using SmartFinance.Repositories;
using SmartFinance.Analyzers;
using SmartFinance.Data;
using SmartFinance.BackgroundJobs;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=app.db"));

//Services
builder.Services.AddHttpClient<FmpService>();
builder.Services.AddScoped<CryptoRepository>();
builder.Services.AddSingleton<StockAnalyzer>();
builder.Services.AddSingleton<SupportedSymbolService>();
builder.Services.AddHostedService<StockBackgroundService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
public partial class Program { }