using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class State 
    { 
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
