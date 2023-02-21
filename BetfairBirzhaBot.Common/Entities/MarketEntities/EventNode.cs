using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class EventNode
    {

        [JsonProperty("event")]
        public Event Event { get; set; }

        [JsonProperty("marketNodes")]
        public List<MarketNode> MarketNodes { get; set; }
    }
}
