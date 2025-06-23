using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Struct;

namespace Nabunassar.Desktops.UserInterfaces.ContextMenus
{
    internal class RadialMenu : ScreenWidget
    {

        const int PanelWidthHeight = 320;
        const int CircleWidthHeight = 64;

        private static Texture2D CircleTexture;
        private static Texture2D CircleTextureFocused;
        private static Texture2D IconTexture;
        private Vector2 _position= Vector2.Zero;

        public RadialMenu(NabunassarGame game,Vector2 position) : base(game)
        {
            _position = position;
            _position.X -= PanelWidthHeight / 2;
            _position.Y-= PanelWidthHeight / 2;
        }

        protected override void LoadContent()
        {
            if (CircleTexture == default)
            {
                CircleTexture = Content.Load<Texture2D>("Assets/Images/Interface/Circle128.png");
                CircleTextureFocused = Content.Load<Texture2D>("Assets/Images/Interface/Circle128_f.png");
                IconTexture = Content.Load<Texture2D>("Assets/Images/Icons/favicon.png");
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        private Panel widget;

        protected override Widget InitWidget()
        {
            var panel = widget = new Panel();
            panel.Width = PanelWidthHeight;
            panel.Height = PanelWidthHeight;

            panel.Top = (int)Math.Floor(_position.Y);
            panel.Left = (int)Math.Floor(_position.X);

            CreateCircle(panel, Direction.Up);
            CreateCircle(panel, Direction.Down);
            CreateCircle(panel, Direction.Left);
            CreateCircle(panel, Direction.Right);
            CreateCircle(panel, Direction.UpLeft);
            CreateCircle(panel, Direction.UpRight);
            CreateCircle(panel, Direction.DownLeft);
            CreateCircle(panel, Direction.DownRight);

            //panel.HorizontalAlignment = HorizontalAlignment.Center;
            //panel.VerticalAlignment = VerticalAlignment.Center;

            return panel;
        }

        private void CreateCircle(Panel panel, Direction direction)
        {
            var x = 0;
            var y = 0;

            void middleX()
            {
                x = PanelWidthHeight / 2 - CircleWidthHeight / 2;
            }

            void downY()
            {
                y = PanelWidthHeight - CircleWidthHeight;
            }

            void middleY()
            {
                y = CircleWidthHeight*2;
            }

            void upHalfPartY()
            {
                y = CircleWidthHeight / 2 + CircleWidthHeight / 4;
            }

            void downHalfPartY()
            {
                upHalfPartY();
                y +=CircleWidthHeight*2;
                y += CircleWidthHeight / 2;
            }

            var xoffset = CircleWidthHeight + CircleWidthHeight / 4;

            void shiftX(bool isPlus=true)
            {
                if (isPlus)
                {
                    x += xoffset;
                }
                else
                {
                    x -= xoffset;
                }
            }

            var fourthPart = CircleWidthHeight / 4;

            bool draw = true;

            switch (direction)
            {
                case Direction.Up:
                    middleX();
                    y+= fourthPart;
                    break;
                case Direction.Down:
                    middleX(); downY();
                    y-= fourthPart;
                    break;
                case Direction.Left:
                    middleY();
                    x += fourthPart;
                    break;
                case Direction.Right:
                    middleY(); x = PanelWidthHeight - CircleWidthHeight;
                    x -= fourthPart;
                    break;
                case Direction.UpLeft:
                    upHalfPartY();
                    middleX();
                    shiftX(false);
                    break;
                case Direction.UpRight:
                    upHalfPartY();
                    middleX();
                    shiftX();
                    break;
                case Direction.DownLeft:
                    middleX();
                    shiftX(false);
                    downHalfPartY();
                    break;
                case Direction.DownRight:
                    middleX();
                    shiftX();
                    downHalfPartY();
                    break;
                default:
                    draw = false;
                    break;
            }

            if (draw)
            {
                var btn = new Button()
                {
                    Background = new TextureRegion(CircleTexture, new Rectangle(0, 0, CircleWidthHeight, CircleWidthHeight)),
                    Width = CircleWidthHeight,
                    Height = CircleWidthHeight
                };

                btn.FocusedBackground = new TextureRegion(CircleTextureFocused, new Rectangle(0, 0, CircleWidthHeight, CircleWidthHeight));
                btn.OverBackground = btn.FocusedBackground;
                btn.PressedBackground = btn.OverBackground;

                btn.Left = x;
                btn.Top = y;

                var icon =  new Image()
                {
                    Renderable = new TextureRegion(IconTexture, new Rectangle(0, 0, 1125, 1125))
                };
                icon.Width = CircleWidthHeight-CircleWidthHeight/4;
                icon.Height = CircleWidthHeight - CircleWidthHeight / 4;
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;

                btn.Content = icon;

                var label = new Label();
                label.Text = direction.ToString();

                label.Left = btn.Left + 5;
                label.Top = btn.Top + 5;

                panel.Widgets.Add(label);
                panel.Widgets.Add(btn);

                panel.Scale = Vector2.One * .001f;
            }
        }


        private TimeSpan prev;
        public override void Update(GameTime gameTime)
        {
            
            if (gameTime.TotalGameTime - prev >= TimeSpan.FromSeconds(1))
            {
                if (widget.Scale.X<1)
                {
                    var plus = 0.005f;
                    widget.Scale = new Vector2(widget.Scale.X + plus, widget.Scale.Y + plus);
                }

                prev = gameTime.ElapsedGameTime;
            }

            if (prev == default)
                prev = gameTime.TotalGameTime;

            base.Update(gameTime);
        }
    }
}
