using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class Score
    {
        [JsonProperty("home")]
        public Participant Home { get; set; }

        [JsonProperty("away")]
        public Participant Away { get; set; }

        [JsonProperty("numberOfYellowCards")]
        public int NumberOfYellowCards { get; set; }

        [JsonProperty("numberOfRedCards")]
        public int NumberOfRedCards { get; set; }

        [JsonProperty("numberOfCards")]
        public int NumberOfCards { get; set; }

        [JsonProperty("numberOfCorners")]
        public int NumberOfCorners { get; set; }

        [JsonProperty("numberOfCornersFirstHalf")]
        public int NumberOfCornersFirstHalf { get; set; }

        [JsonProperty("numberOfCornersSecondHalf")]
        public int NumberOfCornersSecondHalf { get; set; }

        [JsonProperty("bookingPoints")]
        public int BookingPoints { get; set; }
    }

}
