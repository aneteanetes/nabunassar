namespace ioi.Stats.Serialization
{
    internal class ParameterModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public ParameterType Type { get; set; }

        public ParameterModModel[] Modifiers { get; set; }
    }
}
