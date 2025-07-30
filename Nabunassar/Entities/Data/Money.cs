namespace Nabunassar.Entities.Data
{
    public class Money(int gold, int silver, int copper)
    {
        public int Gold { get; private set; } = gold;

        public int Silver { get; private set; } = silver;

        public int Copper { get; private set; } = copper;

        public static Money FromCooper(double copperPile)
        {
            var value = (int)Math.Floor(copperPile);

            var gold = Math.DivRem(value, 1000, out var goldRemind);
            var silver = Math.DivRem(goldRemind, 10, out var silverRemind);
            var copper = silverRemind;

            return new Money(gold, silver, copper);
        }

        public int ToCooper()
        {
            return Gold * 1000 + Silver * 10 + Copper;
        }

        public override string ToString()
        {
            return $"{Gold}g,{Silver}s,{Copper}c";
        }

        public static Money operator -(Money money, Money money2)
        {
            return FromCooper(money.ToCooper() - money2.ToCooper());
        }

        public static Money operator +(Money money, Money money2)
        {
            return FromCooper(money.ToCooper() + money2.ToCooper());
        }

        public static Money operator *(Money a, double percentage)
        {
            var cooperPile = a.ToCooper() * percentage;

            return FromCooper(cooperPile);
        }
    }
}