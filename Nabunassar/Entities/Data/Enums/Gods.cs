namespace Nabunassar.Entities.Data.Enums
{
    internal enum Gods
    {
        Nasho = 1,
        Sabu,
        Rohati,
        Nisa,
        Haya,
        Ailul,
        Tamus,
        Shamadj,
        Aval,
        Ziran,
        Teshrin,
        Nergal,
        //Tiamat = Shamadj
    }

    internal static class GodsExtensions
    {
        public static Color GodColor(this Gods god) => god switch
        {
            Gods.Nasho => "#b8f9f6".AsColor(),
            Gods.Sabu => "#96bb91".AsColor(),
            Gods.Rohati => "#8b7b88".AsColor(),
            Gods.Nisa => "#9c4444".AsColor(),
            Gods.Haya => "#338a00".AsColor(),
            Gods.Ailul => "#ff59a7".AsColor(),
            Gods.Tamus => "#de470b".AsColor(),
            Gods.Shamadj => "#4c718b".AsColor(),
            Gods.Aval => "#f2f3ae".AsColor(),
            Gods.Ziran => "#9598b8".AsColor(),            
            Gods.Teshrin => "#897bd4".AsColor(),
            Gods.Nergal => "#889a7e".AsColor(),
            _ => Color.White,
        };
    }
}
