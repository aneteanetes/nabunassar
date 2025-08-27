using Newtonsoft.Json;

namespace Nabunassar.Entities.Data.Dices
{
    internal class DiceJsonConverter : JsonConverter<Dice>
    {
        public override Dice ReadJson(JsonReader reader, Type objectType, Dice existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;
            return Dice.Parse(s);
        }

        public override void WriteJson(JsonWriter writer, Dice value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
