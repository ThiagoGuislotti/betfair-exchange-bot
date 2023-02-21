using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{
    public class StaticDataFilterModel
    {
        public EFilterCondition Condition { get; set; }
        public double From { get; set; }
        public double To { get; set; }
    }


}
