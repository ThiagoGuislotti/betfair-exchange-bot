using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class AvailableToBack
    {
        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("size")]
        public double Size { get; set; }
    }
}
