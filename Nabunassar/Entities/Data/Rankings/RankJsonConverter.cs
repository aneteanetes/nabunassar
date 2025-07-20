using Geranium.Reflection;
using Nabunassar.Entities.Data.Rankings;
using Newtonsoft.Json;
using System.Reflection;

namespace Nabunassar.Entities.Data.Dices
{
    internal class RankJsonConverter : JsonConverter<Rank>
    {
        public override Rank ReadJson(JsonReader reader, Type objectType, Rank existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            var staticMembers = typeof(Rank).GetMembers(BindingFlags.Static | BindingFlags.Public).OfType<FieldInfo>().ToList();

            var staticMember = staticMembers.FirstOrDefault(x => x.Name == s);

            if(staticMember == null)
                throw new JsonSerializationException($"There is no '{s}' static value in Rank struct!");

            var rankValue = staticMember.GetValue(null);

            return rankValue.As<Rank>();
        }

        public override void WriteJson(JsonWriter writer, Rank value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
