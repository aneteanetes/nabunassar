using Nabunassar.Entities.Data.Rankings;
using Newtonsoft.Json;

namespace Nabunassar.Entities.Data.Dices
{
    internal class RankDiceJsonConverter : JsonConverter<RankDice>
    {
        public override RankDice ReadJson(JsonReader reader, Type objectType, RankDice existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            var diceIdx= s.IndexOf('d');
            if (diceIdx < 0)
                throw new JsonSerializationException($"'{s}' s not valid RankDice expression - 'd' is absent!");

            var rankStr = s.Substring(0,diceIdx);
            var rankNum = int.Parse(rankStr);
            var rank = new Rank(rankNum);

            var diceStr = s.Substring(diceIdx);
            var dice = Dice.Parse(diceStr);

            return new RankDice()
            {
                Rank = rank,
                Dice = dice,
            };
        }

        public override void WriteJson(JsonWriter writer, RankDice value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
