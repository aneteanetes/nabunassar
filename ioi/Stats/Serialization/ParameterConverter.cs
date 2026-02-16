using Newtonsoft.Json;

namespace ioi.Stats.Serialization
{
    internal class ParameterConverter : JsonConverter<Parameter>
    {
        public override Parameter ReadJson(JsonReader reader, Type objectType, Parameter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var model = serializer.Deserialize<ParameterModel>(reader);

            var param = Parameter.CreateValue(model.Key, model.Value, model.Type);

            foreach (var mod in model.Modifiers)
            {                
                param.AddModifier(ParameterModifier.Create(mod.Name,mod.Duration));
            }

            return param;
        }

        public override void WriteJson(JsonWriter writer, Parameter value, JsonSerializer serializer)
        {
            var model = new ParameterModel()
            {
                Key = value.Key,
                Value = value.Get<string>(),
                Modifiers = [..value.GetMods().Select(x=>new ParameterModModel()
                {
                    Name = x.Name,
                    Duration = x.Duration,
                })],
                Type = value.ParameterType
            };

            writer.WriteRaw(serializer.Serialize(model));
        }
    }
}