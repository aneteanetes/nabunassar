namespace Nabunassar.Entities.Data.Enums
{
    internal enum Elements
    {
        Physical,

        Frost,
        Energy,
        Rot,
        Blood,
        Magic,
        Arcane,
        Fire,
        Water,
        Holy,
        Mind,
        Moon,
        Decay,
    }

    internal static class ElementsExtensions
    {
        public static Elements GodElement(this Gods god) => god switch
        {
            Gods.Nasho => Elements.Frost,
            Gods.Sabu => Elements.Energy,
            Gods.Rohati => Elements.Rot,
            Gods.Nisa => Elements.Blood,
            Gods.Haya => Elements.Magic,
            Gods.Ailul => Elements.Arcane,
            Gods.Tamus => Elements.Fire,
            Gods.Shamadj => Elements.Water,
            Gods.Aval => Elements.Holy,
            Gods.Ziran => Elements.Mind,
            Gods.Teshrin => Elements.Moon,
            Gods.Nergal => Elements.Decay,
            _ => Elements.Physical
        };
    }
}
