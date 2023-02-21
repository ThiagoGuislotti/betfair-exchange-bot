using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Core;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Interfaces;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BetfairBirzhaBot.Settings
{

    public class Strategy
    {
        public TimeFilter TimeFilter { get; set; } = new();
        public MarketFilters MarketFilters { get; set; } = new();
        public BetModel Bet { get; set; } = new ();
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<LeagueModel> Leagues { get; set; } = new();

        [JsonIgnore]
        public List<IFilter> Filters { get; set; } = new();

        public StrategySygnalResult CheckStrategy(Game game)
        {
            if (CheckTimeFilter(game) == false)
                return StrategySygnalResult.Failed;

            var filterSygnals = new List<Sygnal>();

            var activeFilters = Filters.Where(f => ((FilterBase)f).IsActive).ToList();

            foreach (var filter in activeFilters)
            {
                var checkResult = filter.Check(game);
                if (checkResult.IsValid)
                    filterSygnals.Add(checkResult);
            }

            if (filterSygnals.Count == activeFilters.Count)
                return new StrategySygnalResult(filterSygnals, Id, game);

            return StrategySygnalResult.Failed;
        }

        private bool CheckTimeFilter(Game game)
        {
            if (TimeFilter.Condition == EFilterCondition.None)
                return true;

            if (game.ElapsedMinutes == -1)
                return false;

            if (TimeFilter.Condition == EFilterCondition.LessOrEquals)
                return game.ElapsedMinutes <= int.Parse(TimeFilter.FromMinutes);

            if (TimeFilter.Condition == EFilterCondition.BiggerOrEquals)
                return game.ElapsedMinutes >= int.Parse(TimeFilter.FromMinutes);

            if (TimeFilter.Condition == EFilterCondition.BiggerOrLess)
                return game.ElapsedMinutes >= int.Parse(TimeFilter.FromMinutes) && game.ElapsedMinutes <= int.Parse(TimeFilter.ToMinutes);

            if (TimeFilter.Condition == EFilterCondition.Equals)
                return game.ElapsedMinutes == int.Parse(TimeFilter.FromMinutes);

            if (TimeFilter.Condition == EFilterCondition.NotEquals)
                return game.ElapsedMinutes != int.Parse(TimeFilter.FromMinutes);

            return false;
        }







    }
}
