using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal readonly struct DiceModifier(int result, int value, DiceModifierType type, DiceOperation operation, Rank rank = default)
    {
        public readonly DiceModifierType Type { get; } = type;

        public readonly int Result { get; } = result;

        public readonly int Value { get; } = value;

        public readonly Rank Rank { get; } = rank;

        public readonly DiceOperation Operation { get; } = operation;

        public override string ToString()
        {
            var operString = " " + Operation.ToOperatorString();

            if (Type != DiceModifierType.RankModified)
                return operString + Result.ToString();
            else
                return $"{Result} = {Rank.Value}{operString} {Value}";
        }

        public string ToFormulaString()
        {
            var game = NabunassarGame.Game;
            var operString = " " + Operation.ToOperatorString();

            if (Type == DiceModifierType.Pure)
            {
                return operString + Result.ToString();
            }
            else if (Type == DiceModifierType.Rank)
            {
                var entity = game.DataBase.GetEntity(Rank.ObjectId);
                return $"{operString} {game.Strings["GameTexts"][nameof(Rank)]}({Rank.Value}) {entity.FormulaName}";
            }
            else
            {
                var entity = game.DataBase.GetEntity(Rank.ObjectId);
                return $"{game.Strings["GameTexts"][nameof(Rank)]}({Rank.Value})  {entity.FormulaName} {operString} {Value}";
            }
        }
    }
}