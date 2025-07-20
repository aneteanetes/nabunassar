namespace Nabunassar.Entities.Data.Dices
{
    internal enum DiceOperation
    {
        Add,
        Substract,
        Multiply,
        Unary
    }

    internal static class DiceOperationExtensions
    {
        public static string ToOperatorString(this DiceOperation operation) => operation switch
        {
            DiceOperation.Add => "+",
            DiceOperation.Substract => "-",
            DiceOperation.Multiply => " * ",
            _ => "",
        };
    }
}
