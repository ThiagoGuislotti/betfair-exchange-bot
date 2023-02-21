using BetfairBirzhaBot.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Common.Helpers
{
    public static class MarketDataHelper
    {
        public static WinMarket FindByTeamType(this List<WinMarket> list, ETeamType type)
        {
            return list.Find(x => x.Type == type);
        }
    }
}
