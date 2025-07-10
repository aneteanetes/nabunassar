using Newtonsoft.Json;

namespace Nabunassar.Entities.Data.Dices
{
    internal class DiceJsonConverter : JsonConverter<Dice>
    {
        public override Dice ReadJson(JsonReader reader, Type objectType, Dice existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            if (s.Length > 0 || !s.Contains("d"))
                throw new JsonSerializationException("Dice expression is wrong! it must be 'dX'");

            if(!int.TryParse(s.Replace("d",""), out var diceNumber))
                throw new JsonSerializationException("Dice expression is wrong! it must be 'dX'");

            return new Dice(diceNumber);
        }

        public override void WriteJson(JsonWriter writer, Dice value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
