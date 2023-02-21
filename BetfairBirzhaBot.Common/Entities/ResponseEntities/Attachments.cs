using Newtonsoft.Json;
using System.Text.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class Attachments
    {
        [JsonProperty("liteMarkets")]
        public Dictionary<string, Market> LiteMarkets { get; set; }

        [JsonProperty("events")]
        public Dictionary<string, MarketEvent> Events { get; set; }
    }

    public class MarketEvent
    {
        [JsonProperty("competitionId")]
        public string CompetitionId { get; set; }

        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
