using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class MarketDetailsResponse
    {
        [JsonProperty("eventTypes")]
        public List<EventType> EventTypes { get; set; }

        public EventType Event => EventTypes.FirstOrDefault();
    }
}
