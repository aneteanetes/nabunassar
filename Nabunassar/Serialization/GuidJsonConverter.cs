using Newtonsoft.Json;

namespace Nabunassar.Serialization
{
    internal class GuidJsonConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                return JsonSerializer.CreateDefault().Deserialize<Guid>(reader);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}