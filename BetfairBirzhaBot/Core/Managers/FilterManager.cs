using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Models;
using BetfairBirzhaBot.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Core
{
    public class FilterManager
    {
        private List<Strategy> _strategies;

        public async Task<List<StrategySygnalResult>> CheckStrategyFilters(Game game)
        {
            var strategySygnals = new List<StrategySygnalResult>();
            var activeStrategies = _strategies.Where(x => x.IsActive).ToList();

            foreach (var strategy in activeStrategies)
            {
                if (strategy.Leagues.Exists(x => x.Name == game.League && x.IncludeToBlacklist))
                    continue;

                var checkResult = strategy.CheckStrategy(game);
                if (checkResult.IsActive)
                    strategySygnals.Add(checkResult);
            }

            return strategySygnals;
        }

        public void SetupStrategies(List<Strategy> strategies)
        {
            _strategies = strategies;
        }

       
    }
}
