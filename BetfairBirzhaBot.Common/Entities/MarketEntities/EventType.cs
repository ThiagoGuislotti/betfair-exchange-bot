using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.MarketEntities
{
    public class EventType
    {

        [JsonProperty("eventNodes")]
        public List<EventNode> EventNodes { get; set; }
        public EventNode EventNode => EventNodes.FirstOrDefault();
    }
}
