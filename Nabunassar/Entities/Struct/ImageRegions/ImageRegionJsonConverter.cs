using Newtonsoft.Json;

namespace Nabunassar.Entities.Struct.ImageRegions
{
    internal class ImageRegionJsonConverter : JsonConverter<ImageRegion>
    {
        /// <summary>
        /// (x,y,width,height)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        /// <exception cref="JsonSerializationException"></exception>
        public override ImageRegion ReadJson(JsonReader reader, Type objectType, ImageRegion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            var str = s.Trim('(', ')');

            var values = str.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            return new ImageRegion(values[0], values[1], values[2], values[3]);
        }

        public override void WriteJson(JsonWriter writer, ImageRegion value, JsonSerializer serializer)
        {
            writer.WriteValue($"({value.X},{value.Y},{value.Width},{value.Height})");
        }
    }
}