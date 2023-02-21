using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class Event
    {
        [JsonProperty("eventName")]
        public string EventName { get; set; }
    }
}
