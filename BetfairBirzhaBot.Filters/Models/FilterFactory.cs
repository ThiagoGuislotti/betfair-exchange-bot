using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Interfaces;

namespace BetfairBirzhaBot.Filters.Models
{
    public class FilterFactory
    {
        private readonly List<EMarket> _resultFilters = new()
        {
            EMarket.WinHome,
            EMarket.WinAway,
            EMarket.Draw,
            EMarket.DoubleChanceBoth,
            EMarket.DoubleChanceWinHome,
            EMarket.DoubleChanceWinAway
        };

        private readonly List<EMarket> _statisticsFilters = new()
        {
            EMarket.Attacks,
            EMarket.DangerousAttacks,
            EMarket.Goals,
            EMarket.KickToGateBorder,
            EMarket.KickToGateDirection,
            EMarket.DoubleChanceWinAway,
            EMarket.Corners,
            EMarket.YellowCards,
            EMarket.RedCards,
        };


        public IFilter CreateDefaultFilter(EMarket type, string filterName = "")
        {
            if (_resultFilters.Contains(type))
                return new ResultsFilter(filterName, type, EFilterCondition.None, false);

            if (type == EMarket.Total)
                return new TotalsFilter(filterName, type, 0, ETotalType.None, false, ETimePart.None, EFilterCondition.None);

            if (type == EMarket.CorrectScore)
                return new CorrectScoreFilter(filterName, type);

            if (type == EMarket.BothToScore)
                return new BothToScoreFilter(filterName, type, EBothToScoreType.NONE, EFilterCondition.None, false);

            if (_statisticsFilters.Contains(type))
                return new StaticDataFilter(filterName, type);

            throw new Exception($"Фильтр {type.ToString()} не реализован.");
        }

        public List<IFilter> CreateDefaultFilters()
        {
            List<IFilter> filters = new List<IFilter>();

            filters.Add(new ResultsFilter("", EMarket.WinHome, EFilterCondition.None, false));
            filters.Add(new ResultsFilter("", EMarket.Draw, EFilterCondition.None, false));
            filters.Add(new ResultsFilter("", EMarket.WinAway, EFilterCondition.None, false));

            filters.Add(new ResultsFilter("", EMarket.WinHome, EFilterCondition.None, true));
            filters.Add(new ResultsFilter("", EMarket.Draw, EFilterCondition.None, true));
            filters.Add(new ResultsFilter("", EMarket.WinAway, EFilterCondition.None, true));


            filters.Add(new BothToScoreFilter("", EMarket.BothToScore, EBothToScoreType.Yes, EFilterCondition.None, false));
            filters.Add(new BothToScoreFilter("", EMarket.BothToScore, EBothToScoreType.No, EFilterCondition.None, false));

            filters.Add(new BothToScoreFilter("", EMarket.BothToScore, EBothToScoreType.Yes, EFilterCondition.None, true));
            filters.Add(new BothToScoreFilter("", EMarket.BothToScore, EBothToScoreType.No, EFilterCondition.None, true));

            return filters;
        }
    }
}
