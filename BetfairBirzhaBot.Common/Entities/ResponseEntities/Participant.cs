using Newtonsoft.Json;

namespace BetfairBirzhaBot.Common.Entities.ResponseEntities
{
    public class Participant
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }

        [JsonProperty("halfTimeScore")]
        public string HalfTimeScore { get; set; }

        [JsonProperty("fullTimeScore")]
        public string FullTimeScore { get; set; }

        [JsonProperty("penaltiesScore")]
        public string PenaltiesScore { get; set; }


        [JsonProperty("games")]
        public string Games { get; set; }

        [JsonProperty("sets")]
        public string Sets { get; set; }

        [JsonProperty("highlight")]
        public bool Highlight { get; set; }

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
