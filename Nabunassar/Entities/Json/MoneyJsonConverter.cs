using Nabunassar.Entities.Game;
using Newtonsoft.Json;

namespace Nabunassar.Entities.Json
{
    internal class MoneyJsonConverter : JsonConverter<Money>
    {
        public override Money ReadJson(JsonReader reader, Type objectType, Money existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = (string)reader.Value;

            if (int.TryParse(value, out var copperPile))
            {
                return Money.FromCooper(copperPile);
            }

            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var g = 0;
            var s = 0;
            var c = 0;

            foreach (var val in values)
            {
                if (Parse(val, "g", out var goldValue))
                    g = goldValue;

                if(Parse(val, "s",out var silverValue))
                    s = silverValue;

                if(Parse(val, "c",out var copperValue))
                    c = copperValue;
            }

            return new Money(g, s, c);
        }

        private bool Parse(string value, string postFix, out int val)
        {
            if (value.Contains(postFix))
            {
                var pureValue = value.Replace(postFix, "").Trim();
                if (int.TryParse(pureValue, out var intValue))
                {
                    val = intValue;
                    return true;
                }
            }

            val = 0;
            return false;
        }

        public override void WriteJson(JsonWriter writer, Money value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
