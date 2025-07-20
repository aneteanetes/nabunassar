using Nabunassar.Entities.Game;
using Nabunassar.Localization;
using Nabunassar.Struct;

namespace Nabunassar
{
    internal static class LocalizedStringsDataExtensions
    {
        static NabunassarGame Game => NabunassarGame.Game;

        public static string GetObjectDescription(this LocalizedStrings strings, GameObject gameObject)
        {
            var file = "ObjectDescriptions";
            var desc = strings[file][gameObject.ObjectId.ToString()].ToString();
            if (desc == Game.Strings.NotFound)
                desc = Game.Strings[file][gameObject.ObjectType.ObjectTypeInLoadedMap()];

            if (desc == Game.Strings.NotFound && gameObject.ObjectType == ObjectType.Ground)
                desc = Game.Strings[file][gameObject.GetPropertyValue<GroundType>(nameof(GroundType)).ToString() + Game.GameState.LoadedMapPostFix];

            return desc;
        }
    }
}