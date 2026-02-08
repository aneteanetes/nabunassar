using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct.FixedCollections.Octas;
using Nabunassar.Struct;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.Combat.SquadView
{
    internal class SquadPlate : ScreenWidget
    {
        private Octa<BattlerAvatar> _avatars = new();
        //private Octa<BattlerHp> _hps = new();
        private SquadInBattle _squad;
        private Side _side;
        private Texture2D _frameImage;
        private Texture2D _frameImageBack;
        public static int SquadPlateWidth = 323;

        public SquadPlate(NabunassarGame game, SquadInBattle squad, Side side) : base(game)
        {
            _squad = squad;
            _side = side;
        }

        public override void LoadContent()
        {
            _frameImage = Content.LoadTexture("Assets/Images/Combat/squad_frame_ver");
            _frameImageBack = Content.LoadTexture("Assets/Images/Combat/squad_frame_hor");
        }

        protected override Widget CreateWidget()
        {
            var panel = new Panel();

            panel.Width = SquadPlateWidth;
            panel.Height = 537;

            var franeImageBack = new Image()
            {
                Renderable = new TextureRegion(_frameImageBack)
            };
            panel.Widgets.Add(franeImageBack);

            for (int i = 0; i < 6; i++)
            {
                var creature = _squad.Squad[i];
                var pos = AvatarPosition(i, _side);

                var avatar = _avatars[i] = new BattlerAvatar(Game, creature, _squad);
                avatar.Left = pos.X;
                avatar.Top = pos.Y;

                //var hp = _hps[i] = new BattlerHp(Game, creature)
                //{
                //    Left = pos.X,
                //    Top = pos.Y + avatar.Height.Value
                //};

                panel.Widgets.Add(avatar);
            }

            var frameImage = new Image()
            {
                Renderable = new TextureRegion(_frameImage)
            };
            panel.Widgets.Add(frameImage);

            //foreach (var hp in _hps)
            //{
            //    panel.Widgets.Add(hp);
            //}

            return panel;
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }

        private static Point AvatarPosition(int octaPosition, Side side)
        {
            static Point getPointByOctaPos(int octaPos)
            {
                return (OctaPosition)octaPos switch
                {
                    OctaPosition.First => new Point(25, 23),
                    OctaPosition.Second => new Point(25, 193),
                    OctaPosition.Third => new Point(25, 363),
                    OctaPosition.Fourth => new Point(173, 23),
                    OctaPosition.Fifth => new Point(173, 193),
                    OctaPosition.Sixth => new Point(173, 363),
                    _ => Point.Zero,
                };
            }

            if (side == Side.Right)
                return getPointByOctaPos(octaPosition);

            return getPointByOctaPos((octaPosition + 3) % 6);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var avatar in _avatars)
            {
                avatar.Update(gameTime);
            }
            //foreach (var hp in _hps)
            //{
            //    hp.Update(gameTime);
            //}
        }
    }
}
