using System.Collections.Generic;

namespace CurrencyConverterAPI.Models;

public class HistoricalRatesResponse
{
    public string Base { get; set; }
    public string Start_date { get; set; }
    public string End_date { get; set; }
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
}
