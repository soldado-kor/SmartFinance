using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SmartFinance.Repositories;
using SmartFinance.Services;
using SmartFinance.Models;
using SmartFinance.Analyzers;
using SmartFinance.Exceptions;
using SmartFinance.DTOs;

namespace SmartFinance.Controllers;

[ApiController]
[Route("api/crypto")]
public class CryptoController : ControllerBase
{
    private readonly FmpService _service;
    private readonly CryptoRepository _repo;
    private readonly StockAnalyzer _analyzer;
    private readonly SupportedSymbolService _supportedSymbolService;

    public CryptoController(FmpService service, CryptoRepository repo, StockAnalyzer analyzer, SupportedSymbolService supportedSymbolService)
    {
        _service = service;
        _repo = repo;
        _analyzer = analyzer;
        _supportedSymbolService = supportedSymbolService;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync(SyncRequestDto request)
    {
        var symbol = request.Symbol.ToLower();
        try
        {
            var price = await _service.GetStockPrice(symbol);

            await _repo.AddAsync(new CryptoPrice
            {
                Symbol = symbol.ToLower(),
                Price = price,
                Timestamp = DateTime.UtcNow
            });

        return Ok(new SyncResponseDto
        {
            Message = "Crypto synced", 
            Price=price
        });
        }

    catch (UnsupportedSymbolException e)
    {
        return BadRequest(e.Message);
    }
    catch (NoApiDataException e)
    {
        return StatusCode(503, e.Message);
    }
        catch (Exception)
    {
        return StatusCode(500);
    }
    }

    [HttpGet("insights/{symbol}")]
    public IActionResult Insights(string symbol)
    {
        var data = _repo.GetBySymbol(symbol.ToLower());

        var result = _analyzer.Analyze(data);

        return Ok(new AnalysisResponseDTO
    {
        Trend = result.Trend,
        ChangePercent = result.ChangePercent,
        HighestPrice = result.HighestPrice,
        HighestPriceTimestamp = result.HighestPriceTimestamp,
        LowestPrice = result.LowestPrice,
        LowestPriceTimestamp = result.LowestPriceTimestamp
    });
    }

     [HttpGet("analysis/{symbol}")]

    public IActionResult Analysis(string symbol, [FromQuery] [Range(1, 365, ErrorMessage = "Days must be between 1 and 365.")] int days)
    {
        symbol = symbol.ToLower();
            if (!_supportedSymbolService.IsSupported(symbol))
        {
            return BadRequest($"Symbol {symbol} is not supported.");
        }

        DateTime endTime = DateTime.UtcNow;
        DateTime startTime = endTime.AddDays(-days);

        var data = _repo.GetBySymbolAndDateRange(symbol, startTime, endTime);
        if (data.Count == 0)
        {
            return NotFound($"No data found for symbol {symbol} in the last {days} days.");
        }

        var result = _analyzer.Analyze(data);

        return Ok(new AnalysisResponseDTO
        {
            Trend = result.Trend,
            ChangePercent = result.ChangePercent,
            HighestPrice = result.HighestPrice,
            HighestPriceTimestamp = result.HighestPriceTimestamp,
            LowestPrice = result.LowestPrice,
            LowestPriceTimestamp = result.LowestPriceTimestamp,
            StartTime = startTime,
            EndTime = endTime
        });
    }
}