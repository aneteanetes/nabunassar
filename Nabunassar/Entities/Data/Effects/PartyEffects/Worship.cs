using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Entities.Data.Effects.PartyEffects
{
    internal class Worship : BaseEffect
    {
        public Worship(NabunassarGame game) : base(game)
        {
        }

        public override EffectType Type => EffectType.Boon;

        public override string IconPath => "Assets/Images/Icons/Effects/sun.png";
        public override Color IconColor => Color.Yellow;

        public override Description GetDescription()
        {
            var data = Description.Create(Game.Strings["GameTexts"]["Worship"].ToString());
            data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["PartyEffect"], Color.Gray);

            var text = $"{Game.Strings["Effects/EffectDescriptions"]["Worship"]}: {Game.GameState.Prayers.PrayerCounter}/100";

            data.Append(DescriptionPosition.Center, text);

            return data;
        }

        public override void Update(GameTime gameTime)
        {
            Charges = Game.GameState.Prayers.PrayerCounter;
        }
    }
}