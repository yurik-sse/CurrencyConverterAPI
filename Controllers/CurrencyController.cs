using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private static readonly string[] ExcludedCurrencies = ["TRY", "PLN", "THB", "MXN"];

    public CurrencyController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRates(string baseCurrency = "EUR")
    {
        string cacheKey = $"latest-{baseCurrency}";
        if (!_cache.TryGetValue(cacheKey, out object cachedRates))
        {
            var client = _httpClientFactory.CreateClient("FrankfurterApi");
            var response = await client.GetAsync($"latest?base={baseCurrency}");
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            cachedRates = await response.Content.ReadFromJsonAsync<object>();
            _cache.Set(cacheKey, cachedRates, TimeSpan.FromHours(1));
        }

        return Ok(cachedRates);
    }

    [HttpPost("convert")]
    public async Task<IActionResult> ConvertCurrency(string from, string to, decimal amount)
    {
        if (ExcludedCurrencies.Contains(from) || ExcludedCurrencies.Contains(to))
            return BadRequest("Currency conversion not allowed for TRY, PLN, THB, MXN.");

        var client = _httpClientFactory.CreateClient("FrankfurterApi");

        var policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var response = await policy.ExecuteAsync(() =>
            client.GetAsync($"latest?base={from}&symbols={to}"));

        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

        var rateData = await response.Content.ReadFromJsonAsync<ExchangeRateResponse>();
        if (!rateData.Rates.TryGetValue(to, out var rate))
            return NotFound("Exchange rate not found.");

        return Ok(new { from, to, originalAmount = amount, convertedAmount = Math.Round(amount * rate, 2) });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistoricalRates(string baseCurrency, string start, string end, int page = 1, int pageSize = 20)
    {
        var client = _httpClientFactory.CreateClient("FrankfurterApi");
        var response = await client.GetAsync($"{start}..{end}?base={baseCurrency}");
        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

        var historicalData = await response.Content.ReadFromJsonAsync<HistoricalRatesResponse>();

        var paginatedRates = historicalData.Rates
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToDictionary(x => x.Key, x => x.Value);

        return Ok(new
        {
            baseCurrency,
            start,
            end,
            page,
            pageSize,
            totalRecords = historicalData.Rates.Count,
            rates = paginatedRates
        });
    }
}
