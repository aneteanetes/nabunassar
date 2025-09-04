using Geranium.Reflection;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Game;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Stats
{
    internal abstract class BaseStat<T>
        where T : BaseStat<T>
    {
        protected Creature Creature { get; }

        public BaseStat(Creature creature=null)
        {
            Creature = creature;
        }

        private static T _instance;
        public static T Instance
        {
            get
            {
                _instance ??= typeof(T).New((Creature)null).As<T>();

                return _instance;
            }
        }

        public abstract string GetName(LocalizedStrings strings);

        public abstract Color GetNameColor();

        public abstract void Build(DescriptionBuilder builder, LocalizedStrings strings);

        public Description GetDescription(NabunassarGame game)
        {
            var strings = game.Strings;
            var builder = Description.Create(GetName(strings), GetNameColor());
            Build(builder, strings);

            return builder;
        }
    }
}
