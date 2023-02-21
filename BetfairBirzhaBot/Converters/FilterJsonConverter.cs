using BetfairBirzhaBot.Filters.Interfaces;
using BetfairBirzhaBot.Filters.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BetfairBirzhaBot.Converters
{
    public class JsonFilterConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool result = objectType == typeof(IFilter);
            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //var filterBase = (FilterBase)reader.Value;

            //Type neededFilterType = null;

            //if (filterBase.Group == Filters.Enums.EFilterGroup.Results)
            //    neededFilterType = typeof(ResultsFilter);

            //if (filterBase.Group == Filters.Enums.EFilterGroup.Static)
            //    neededFilterType = typeof(StaticDataFilter);

            //if (filterBase.Group == Filters.Enums.EFilterGroup.Total)
            //    neededFilterType = typeof(TotalsFilter);

            //if (filterBase.Group == Filters.Enums.EFilterGroup.CorrectScore)
            //    neededFilterType = typeof(CorrectScoreFilter);

            return serializer.Deserialize(reader, typeof(FilterBase));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
