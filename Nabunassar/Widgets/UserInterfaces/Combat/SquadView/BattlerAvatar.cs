using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.UserInterfaces.Combat.SquadView
{
    internal class BattlerAvatar : Panel, IFeatured
    {
        private Panel _red;
        private Creature _creature;

        private int ImageAreaHeight = 150;
        private int DelimiterHeight = 20;
        private Label _text;

        public BattlerAvatar(NabunassarGame game, Creature creature, SquadInBattle squad)
        {
            _creature = creature;

            this.Width = 130;
            this.Height = ImageAreaHeight+ DelimiterHeight;

            var emptyTexture = game.Content.LoadTexture("Assets/Images/Combat/empty-battler");


            SpriteEffects getFlipInfo()
            {
                if (squad.IsPlayerSquad && squad.Side == Struct.Side.Right)
                    return SpriteEffects.FlipHorizontally;

                if (!squad.IsPlayerSquad && squad.Side == Struct.Side.Left)
                    return SpriteEffects.FlipHorizontally;

                return SpriteEffects.None;
            }

            var img = new Image
            {
                Renderable = new TextureRegion(creature ==null ? emptyTexture : game.Content.LoadTexture(creature.PortraitBattle))
                {
                    SpriteEffects = getFlipInfo()
                },
                Height = ImageAreaHeight,
                Width = this.Width
            };

            this.Widgets.Add(img);

            if (creature != null)
            {
                _red = new Panel
                {
                    Background = new SolidBrush(Color.Red.SetAlpha(.5f)),
                    Width = this.Width,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                _red.Height = 0;

                this.Widgets.Add(_red);

                _text = new Label()
                {
                    Text = GetHp(),
                    Font = game.Content.LoadFont(Fonts.Retron).GetFont(18),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                this.Widgets.Add(_text);
            }
        }

        private string GetHp() => $"{_creature.HPNow} / {_creature.HPMax}";

        public void Update(GameTime gameTime)
        {
            if (_creature == null)
                return;

            _red.Height = ImageAreaHeight * ((_creature.HPNowPercent/100)/100);
            _text.Text = GetHp();
        }
    }
}