using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Formulas
{
    internal static class SkillCheck
    {
        public static bool Roll(Rank checkRank, Dice checkDice, Rank skillRank, Dice skillDice, Dice characteristicdDice)
        {
            //var @lock = ((int)Rank.Advanced) * 2 + Dice.d12;
            //var user = ((int)Rank.Advanced) + Dice.d12 + Dice.d8;

            var checkValue = ((int)checkRank) * 2 + checkDice;
            var skillValue = ((int)skillRank) + skillDice + characteristicdDice;


            return skillValue >= checkValue;
        }
    }
}
