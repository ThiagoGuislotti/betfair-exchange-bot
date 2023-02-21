using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class EventTimeline
    {
        [JsonProperty("score")]
        public Score Score { get; set; }

        [JsonProperty("elapsedRegularTime")]
        public int ElapsedRegularTime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("inPlayMatchStatus")]
        public string InPlayMatchStatus { get; set; }
    }
}
