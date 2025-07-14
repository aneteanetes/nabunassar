namespace Nabunassar.Entities.Data.Dices
{
    internal readonly struct DiceModifier(int value, DiceModifierType type)
    {
        public readonly DiceModifierType Type { get; } = type;

        public readonly int Value { get; } = value;
    }
}