using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class Exchange
    {
        [JsonProperty("availableToBack")]
        public List<AvailableToBack> AvailableToBack { get; set; }
    }
}
