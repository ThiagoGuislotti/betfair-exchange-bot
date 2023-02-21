using BetfairBirzhaBot.Common.Entities;
using BetfairBirzhaBot.Filters.Enums;
using BetfairBirzhaBot.Filters.Interfaces;
using BetfairBirzhaBot.Utilities;

namespace BetfairBirzhaBot.Filters.Models
{
    public abstract class FilterBase : IFilter
    {
        public bool IsActive { get; set; }
        public bool IsPrematch { get; set; }
        public string Id { get; set; }

        public string Name { get; set; }
        public EMarket Type { get; set; }
        public EFilterGroup Group { get; set; }
        public EFilterCondition Condition { get; set; }
        public abstract Sygnal Check(Game game);

        public FilterBase(string name, EMarket type)
        {
            Name = name;
            Type = type;
            Id = Guid.NewGuid().ToString();

        }


        public Sygnal CheckConditions(double coefficient, double from, double to, EFilterCondition condition = EFilterCondition.None)
        {
            EFilterCondition cond;

            if (condition != EFilterCondition.None)
                cond = condition;
            else
                cond = Condition;

            if (cond == EFilterCondition.NotEquals)
                if (coefficient != from)
                    return CreateSygnal(coefficient, from, to);

            if (cond == EFilterCondition.LessOrEquals)
                if (coefficient <= from)
                    return CreateSygnal(coefficient, from, to);

            if (cond == EFilterCondition.BiggerOrEquals)
                if (coefficient >= from)
                    return CreateSygnal(coefficient, from, to);

            if (cond == EFilterCondition.BiggerOrLess)
                if (coefficient >= from && coefficient <= to)
                    return CreateSygnal(coefficient, from, to);

            if (cond == EFilterCondition.Equals)
                if (coefficient == from)
                    return CreateSygnal(coefficient, from, to);

            return new Sygnal();
        }

        public Sygnal CreateSygnal(double c, double f, double t = 0)
        {
            return new Sygnal(Type, Name, c, Condition, f, t);
        }




    }


}
