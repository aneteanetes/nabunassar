namespace ioi.Entities.Game
{
    internal class Merchant : GameObject
    {
        public Merchant()
        {
            ObjectType = ioi.Struct.ObjectType.Merchant;
        }

        public double BuyPercent { get; set; }

        public double SellPercent { get; set; }
    }
}
