using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;

namespace BetfairBirzhaBot.Filters.Models
{
    public class StaticDataFilter : FilterBase
    {
        public StaticDataFilterModel Home { get; set; } = new();
        public StaticDataFilterModel Away { get; set; } = new();
        public StaticDataFilterModel Both { get; set; } = new();
        public bool SecondModeActive { get; set; } = false;
        public EFilterCondition SecondModeCondition { get; set; } = EFilterCondition.None;
        public int SecondModeValue { get; set; } = 0;
        public StaticDataFilter(string name, EMarket type) : base(name, type)
        {
            Group = EFilterGroup.Static;
        }

        public override Sygnal Check(Game game)
        {
            if (game.Statistics is null)
                return Sygnal.Fail;
            
            var stats = game.Statistics.Statistics;
            
            

            if (Type == EMarket.Goals)
                return CheckFilterModel(stats.HomeGoals, stats.AwayGoals);

            if (Type == EMarket.Attacks)
                return CheckFilterModel(stats.HomeAttacks, stats.AwayAttacks);

            if (Type == EMarket.DangerousAttacks)
                return CheckFilterModel(stats.HomeDangerousAttacks, stats.AwayDangerousAttacks);

            if (Type == EMarket.KickToGateBorder)
                return CheckFilterModel(stats.HomeShotsOnTarget, stats.AwayShotsOnTarget);

            if (Type == EMarket.KickToGateDirection)
                return CheckFilterModel(stats.HomeShotsOffTarget, stats.AwayShotsOffTarget);

            if (Type == EMarket.RedCards)
                return CheckFilterModel(stats.HomeRedCards, stats.AwayRedCards);

            if (Type == EMarket.YellowCards)
                return CheckFilterModel(stats.HomeYellowCards, stats.AwayYellowCards);

            if (Type == EMarket.Corners)
                return CheckFilterModel(stats.HomeCorners, stats.AwayCorners);

            return Sygnal.Fail;
        }

        private Sygnal CheckFilterModel(int homePoints, int awayPoints)
        {
            var sygnals = new List<Sygnal>();

            if (SecondModeActive)
            {
                int difference = homePoints - awayPoints;

                if (SecondModeCondition == EFilterCondition.Less)
                    if (difference < 0 && Math.Abs(difference) > SecondModeValue)
                        return CreateSygnal(homePoints, awayPoints);

                if (SecondModeCondition == EFilterCondition.LessOrEquals)
                    if (difference < 0 && Math.Abs(difference) == SecondModeValue)
                        return CreateSygnal(homePoints, awayPoints);

                if (SecondModeCondition == EFilterCondition.Bigger)
                    if (difference > SecondModeValue)
                        return CreateSygnal(homePoints, awayPoints);

                if (SecondModeCondition == EFilterCondition.BiggerOrEquals)
                    if (difference == SecondModeValue)
                        return CreateSygnal(homePoints, awayPoints);

                if (SecondModeCondition == EFilterCondition.NotEquals)
                    if (homePoints != awayPoints)
                        return CreateSygnal(homePoints, awayPoints);

                if (SecondModeCondition == EFilterCondition.Equals)
                    if (homePoints == awayPoints)
                        return CreateSygnal(homePoints, awayPoints);
            }
            else
            {
                if (Home.Condition != EFilterCondition.None)
                    sygnals.Add(CheckConditions(homePoints, Home.From, Home.To, Home.Condition));
                if (Away.Condition != EFilterCondition.None)
                    sygnals.Add(CheckConditions(awayPoints, Away.From, Away.To, Away.Condition));
                if (Both.Condition != EFilterCondition.None)
                    sygnals.Add(CheckConditions(homePoints + awayPoints, Both.From, Both.To, Both.Condition));

                int neededSygnals = 0;
                if (Home.Condition != EFilterCondition.None)
                    neededSygnals++;
                if (Away.Condition != EFilterCondition.None)
                    neededSygnals++;
                if (Both.Condition != EFilterCondition.None)
                    neededSygnals++;

                return sygnals.Where(x => x.IsValid).ToList().Count == neededSygnals ? sygnals.FirstOrDefault() : Sygnal.Fail;

            }



            return default;
        }


    }


}
