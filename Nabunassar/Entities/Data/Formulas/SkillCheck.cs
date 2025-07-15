using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Formulas
{
    internal static class SkillCheck
    {
        /*
        Цель: Ранг_Замка* N + Уровень_Замка (d4/d6/d8/d10/d12)
        Бросок: Ранг_Умения + Кубик_Умения + Кубик_Характеристики


        Ранг_Замка - 2
        Уровень_Замка - d8

        Ранг_Умения = 2
        Кубик_Умения = d12
        Кубик_Характеристики = d8
        */

        public static RollResult Roll(Rank checkRank, Dice checkDice, Rank skillRank, Dice skillDice, Dice characteristicdDice)
        {
            //var @lock = ((int)Rank.Advanced) * 2 + Dice.d12;
            //var user = ((int)Rank.Advanced) + Dice.d12 + Dice.d8;

            var checkValue = checkRank * 2 + checkDice;
            var skillValue = skillRank + skillDice + characteristicdDice;

            return new RollResult()
            {
                Complexity = checkValue,
                Roll = skillValue,
                IsSuccess = skillValue.Result >= checkValue.Result
            };
        }
    }
}
