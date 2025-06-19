using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Currency.Service
{
    public class ExchangeRateResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, double> Rates { get; set; }
    }
}
