using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class Description
    {
        [JsonProperty("marketName")]
        public string MarketName { get; set; }

        [JsonProperty("marketType")]
        public string MarketType { get; set; }
    }
}
