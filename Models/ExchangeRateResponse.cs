using System.Collections.Generic;

namespace CurrencyConverterAPI.Models;

public class ExchangeRateResponse
{
    public string Base { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}
