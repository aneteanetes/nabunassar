using Geranium.Reflection;

namespace ioi.Stats
{
    public abstract class ParameterModifier
    {
        private static Dictionary<string, Type> registeredMods = [];

        public string Name =>this.GetType().Name;

        public ModifierDuration Duration { get; set; }

        public virtual T Get<T>(T value) => value;

        public virtual T Set<T>(T value) => value;

        public virtual void Tick() { }

        public static void Register<TMod>()
        {
            registeredMods.Add(typeof(TMod).Name, typeof(TMod));
        }

        public static ParameterModifier Create(string name, ModifierDuration modifierDuration)
        {
            if (registeredMods.TryGetValue(name, out var modifierType))
                return modifierType.NewAs<ParameterModifier>();

            return default;
        }
    }
}
