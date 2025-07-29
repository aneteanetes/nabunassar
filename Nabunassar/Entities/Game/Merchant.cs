namespace Nabunassar.Entities.Game
{
    internal class Merchant : GameObject
    {
        public Merchant()
        {
            ObjectType = Nabunassar.Struct.ObjectType.Merchant;
        }

        public double BuyPercent { get; set; }

        public double SellPercent { get; set; }
    }
}
