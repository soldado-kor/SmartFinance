using System.Net.Http.Json;
using SmartFinance.Exceptions; 

namespace SmartFinance.Services;

public class FmpService
{
    private readonly HttpClient _http;

    public FmpService(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
    }

    public async Task<decimal> GetStockPrice(string symbol)
    {
        var id = symbol.ToLower() switch
        {
            "btc" => "bitcoin",
            "eth" => "ethereum",
            _ => throw new UnsupportedSymbolException(symbol)
        };

        var url = $"https://api.coingecko.com/api/v3/simple/price?ids={id}&vs_currencies=usd";

        var httpResponse = await _http.GetAsync(url);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception($"API error: {httpResponse.StatusCode} - {error}");
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<Dictionary<string, Crypto>>();

        if (response == null)
            throw new NoApiDataException();

        return response[id].usd;
    }
}

public class Crypto
{
    public decimal usd { get; set; }
}