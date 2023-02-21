using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{
    public class TotalsFilter : FilterBase
    {
        public double TotalParameter { get; set; }
        public ETotalType TotalType { get; set; }
        public ETimePart Part { get; set; }
        public double From { get; set; }
        public double To { get; set; }

        public TotalsFilter(string name, EMarket type, double totalParameter, ETotalType totalType, bool isPrematch, ETimePart timePart, EFilterCondition condition, double from = 1, double to = 1) : base(name, type)
        {
            Condition = condition;
            TotalParameter = totalParameter;
            TotalType = totalType;
            Part = timePart;
            From = from;
            To = to;
            Group = EFilterGroup.Total;
            IsPrematch = isPrematch;
        }

        public override Sygnal Check(Game game)
        {
            if (TotalType == ETotalType.None)
                return Sygnal.Fail;

            if (Part == ETimePart.None)
                Part = ETimePart.FullGame;

            List<TotalMarket> totalMarkets = null;
            
            if (Part == ETimePart.FirstHalf)
                totalMarkets = IsPrematch ? game.TotalMarketsFirstHalfStart : game.TotalMarketsFirstHalfCurrent;
            if (Part == ETimePart.FullGame)
                totalMarkets = IsPrematch ? game.TotalMarketsStart : game.TotalMarketsCurrent;

            var totalMarket = totalMarkets.Find(x => x.Parameter == TotalParameter);
            if (totalMarket is null)
                return Sygnal.Fail;
            

            TotalMarketData totalData = null;


            totalData = TotalType == ETotalType.Over ? totalMarket.Over : totalMarket.Under;

            var sygnal = CheckConditions(totalData.Coefficient, From, To);

            if (sygnal.IsValid)
            {
                sygnal.TotalParameter = TotalParameter;
                sygnal.TotalType = TotalType;
                sygnal.MarketId = totalMarket.MarketId;
                sygnal.SelectionId = totalData.SelectionId;
            }
            


            return sygnal;
         }
    }
}
