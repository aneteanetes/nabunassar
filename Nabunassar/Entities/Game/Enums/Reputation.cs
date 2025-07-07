namespace Nabunassar.Entities.Game.Enums
{
    internal enum Reputation
    {
        Hated,
        Hostile,
        Unfriendly,
        Neutral,
        Friendly,
        Honored,
        Revered,
        Exalted
    }

    internal static class ReputationExtensions
    {
        static NabunassarGame Game => NabunassarGame.Game;

        public static string Name(this Reputation rep)
        {
            return Game.Strings["Enums/Reputation"][rep.ToString()];
        }

        public static Color Color(this Reputation rep)
        {
            var hex = Game.DataBase.GetFromDictionary<string>("Data/Enums/ReputationColor.json", rep.ToString());
            return hex.AsColor();
        }
    }
}