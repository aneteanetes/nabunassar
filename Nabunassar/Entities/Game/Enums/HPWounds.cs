namespace Nabunassar.Entities.Game.Enums
{
    internal enum HPWounds
    {
        Uninjured,
        SlightlyWounded,
        Wounded,
        BadlyWounded,
        GravelyWounded,
        NearDeath,
        Dead
    }

    internal static class HPWoundsExtensions
    {
        static NabunassarGame Game => NabunassarGame.Game;

        public static string WoundName(this HPWounds wound)
        {
            return Game.Strings["HPWounds/WoundNames"][wound.ToString()];
        }

        public static Color WoundColor(this HPWounds wound)
        {
            var hex = Game.DataBase.GetFromDictionary<string>("Data/HPWounds/WoundColors.json", wound.ToString());
            return hex.AsColor();
        }
    }
}