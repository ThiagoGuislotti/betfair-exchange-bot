using BetfairBirzhaBot.Filters.Models;
using System.Collections.Generic;

namespace BetfairBirzhaBot.Settings
{
    public class MarketFilters
    {
        public List<ResultsFilter> ResultsFilters { get; set; } = new();
        public List<StaticDataFilter> StaticFilters { get; set; } = new();
        public List<TotalsFilter> TotalFilters { get; set; } = new();
        public List<CorrectScoreFilter> CorrectScoreFilters { get; set; } = new();
        public List<BothToScoreFilter> BothToScoreFilters { get; set; } = new();
    }
}
