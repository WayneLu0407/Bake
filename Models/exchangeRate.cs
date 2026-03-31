using System.Text.Json.Serialization;

namespace Bake.Models
{
    internal class exchangeRate
    {
        public string Date { get; set; }
        [JsonPropertyName("USD/NTD")]
        public string USDNTD { get; set; }
        public string RMBNTD { get; set; }
        public string EURUSD { get; set; }
        public string USDJPY { get; set; }
        public string GBPUSD { get; set; }
        public string AUDUSD { get; set; }
        public string USDHKD { get; set; }
        public string USDRMB { get; set; }
        public string USDZAR { get; set; }
        public string NZDUSD { get; set; }
    }
}