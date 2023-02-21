using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Settings
{
    public class TimeFilter
    {
        public bool IsActive { get; set; }
        public string FromMinutes { get; set; }
        public string ToMinutes { get; set; }
        public EFilterCondition Condition { get; set; }

        public bool Check(Game game)
        {
            if (!IsActive)
                return true;

            return false;
        }
    }
}
