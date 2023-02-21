using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Models;

namespace BetfairBirzhaBot.Filters.Interfaces
{
    public interface IFilter
    {
        string Id { get; set; }
        string Name { get; set; }
        EMarket Type { get; set; }
        EFilterGroup Group { get; set; }
        Sygnal Check(Game game);
    }
}
