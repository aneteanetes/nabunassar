using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Extensions.OrthographCameraExtensions;
using Nabunassar.Struct;
using Nabunassar.Systems;
using GameObject = Nabunassar.Entities.Game.GameObject;

namespace Nabunassar.Desktops.UserInterfaces.ContextMenus
{
    internal class RadialMenu : ScreenWidget
    {
        const int PanelWidthHeight = 320;
        const int CircleWidthHeight = 64;
        const int CenterCircleWidthHeight = 140;

        private static Dictionary<string, TextureRegion> ActionIcons = new();
        private static Texture2D CircleTexture;
        private static Texture2D MainTileset;
        private static Texture2D CircleTextureFocused;
        private static Texture2D IconTexture;
        private static Texture2D CenterCircleTexture;
        private Vector2 _position= Vector2.Zero;

        public static bool IsContextMenuOpened { get; set; }

        protected override bool IsMouseActiveOnRootWidget => false;

        private GameObject _gameObject;
        private Texture2D _gameObjectImage;

        public RadialMenu(NabunassarGame game, GameObject gameObject, Vector2 position) : base(game)
        {
            _gameObject = gameObject;
            _position = position;
        }

        public static void Open(NabunassarGame game, GameObject gameObject, Vector2 position)
        {
            game.SwitchDesktop(new RadialMenu(game, gameObject, position));
            IsContextMenuOpened = true;
        }

        protected override void LoadContent()
        {
            if (CircleTexture == default)
            {
                CircleTexture = Content.Load<Texture2D>("Assets/Images/Interface/Circle64.png");
                CircleTextureFocused = Content.Load<Texture2D>("Assets/Images/Interface/Circle64_f.png");
                IconTexture = Content.Load<Texture2D>("Assets/Images/Icons/favicon.png");
                CenterCircleTexture = Content.Load<Texture2D>("Assets/Images/Interface/Circle140_f.png");
                MainTileset = Content.Load<Texture2D>("Assets/Tilesets/colored-transparent_packed.png");

                ActionIcons["speak"] = new TextureRegion(MainTileset, new Rectangle(576, 208, 16, 16));
            }

            if (_gameObject!=default && _gameObject.Image != default)
                _gameObjectImage = Content.Load<Texture2D>(_gameObject.Image);
        }

        protected override void UnloadContent()
        {
            _gameObjectImage = null;
            base.UnloadContent();
        }

        private Panel widget;
        private Image centerImage;
        protected override Widget InitWidget()
        {
            Game.DisableSystems(typeof(PlayerControllSystem),typeof(FocusSystem));

            var globalPanel = new Panel();

            var panel = widget = new Panel();
            panel.Width = PanelWidthHeight;
            panel.Height = PanelWidthHeight;

            globalPanel.Widgets.Add(panel);

            panel.Top = (int)Math.Floor(_position.Y- PanelWidthHeight / 2);
            panel.Left = (int)Math.Floor(_position.X- PanelWidthHeight / 2);

            TextureRegion imageRegion = default;
            if (_gameObjectImage == null)
            {
                imageRegion = new TextureRegion(IconTexture, new Rectangle(0, 0, IconTexture.Width, IconTexture.Height));
            }
            else
            {
                imageRegion = new TextureRegion(_gameObjectImage, new Rectangle(0, 0, _gameObjectImage.Width, _gameObjectImage.Height));
            }

            centerImage = new Image()
            {
                Renderable = imageRegion,
                Width = 140,
                Height = 140
            };
            centerImage.MouseEntered += Btn_MouseEntered;
            centerImage.MouseLeft += Btn_MouseLeft;
            centerImage.HorizontalAlignment = HorizontalAlignment.Center;
            centerImage.VerticalAlignment = VerticalAlignment.Center;
            //centerImage.TouchDown += CenterCircle_TouchDown;
            panel.Widgets.Add(centerImage);

            var centerCircle = new Image()
            {
                Renderable = new TextureRegion(CenterCircleTexture, new Rectangle(0, 0, CenterCircleWidthHeight, CenterCircleWidthHeight)),
                Width = CenterCircleWidthHeight,
                Height = CenterCircleWidthHeight
            };
            centerCircle.MouseEntered += Btn_MouseEntered;
            centerCircle.MouseLeft += Btn_MouseLeft;
            centerCircle.HorizontalAlignment = HorizontalAlignment.Center;
            centerCircle.VerticalAlignment = VerticalAlignment.Center;
            //centerCircle.TouchDown += CenterCircle_TouchDown;
            panel.Widgets.Add(centerCircle);

            FillActionsBasedOnObjectType(panel);
            globalPanel.TouchDown += GlobalPanel_TouchDown;

            //var icon = new TextureRegion(IconTexture, new Rectangle(0, 0, 1125, 1125));
            //CreateCircle(panel, Direction.Up, icon);
            //CreateCircle(panel, Direction.Down, icon);
            //CreateCircle(panel, Direction.Left, icon);
            //CreateCircle(panel, Direction.Right, icon);
            //CreateCircle(panel, Direction.UpLeft, icon);
            //CreateCircle(panel, Direction.UpRight, icon);
            //CreateCircle(panel, Direction.DownLeft, icon);
            //CreateCircle(panel, Direction.DownRight, icon);

            return globalPanel;
        }

        private void GlobalPanel_TouchDown(object sender, EventArgs e)
        {
            QueueForClosing = true;
        }

        private void FillActionsBasedOnObjectType(Panel panel)
        {
            if (_gameObject == null)
            {

                return;
            }

            switch (_gameObject.ObjectType)
            {
                case ObjectType.NPC:
                    FillNPCActions(panel);
                    break;
                default:
                    break;
            }
        }

        private void FillNPCActions(Panel panel)
        {
            CreateCircle(panel, Direction.Up, ActionIcons["speak"], TalkWithNPC);
        }

        private void TalkWithNPC()
        {
            Close();
            Game.SwitchDesktop(new DialogueMenu(Game, _gameObject));
        }

        private void CenterCircle_TouchDown(object sender, EventArgs e)
        {
            Close();
        }

        public override void Close()
        {
            Game.EnableSystems();
            IsContextMenuOpened = false;
            PlayerControllSystem.IsPreventNextMove = true;
            base.Close();
        }

        private void CreateCircle(Panel panel, Direction direction, TextureRegion iconTexture, Action click=default)
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

                btn.MouseEntered += Btn_MouseEntered;
                btn.MouseLeft += Btn_MouseLeft;
                btn.TouchDown += (s, e) => QueueForClosing = false; //здесь можно отменить закрытие по клику
                btn.Click += (s, e) => click?.Invoke(); // а здесь обрабатывается событие по клику на кнопке
                btn.FocusedBackground = new TextureRegion(CircleTextureFocused, new Rectangle(0, 0, CircleWidthHeight, CircleWidthHeight));
                btn.OverBackground = btn.FocusedBackground;
                btn.PressedBackground = btn.OverBackground;

                btn.Left = x;
                btn.Top = y;

                var icon =  new Image()
                {
                    Renderable = iconTexture
                };
                icon.Width = CircleWidthHeight-CircleWidthHeight/4;
                icon.Height = CircleWidthHeight - CircleWidthHeight / 4;
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;

                btn.Content = icon;

                //var label = new Label();
                //label.Text = direction.ToString();

                //label.Left = btn.Left + 5;
                //label.Top = btn.Top + 5;

                //panel.Widgets.Add(label);
                panel.Widgets.Add(btn);
                btn.BringToFront();

                panel.Scale = Vector2.One * .001f;
            }
        }

        private void Btn_MouseLeft(object sender, EventArgs e)
        {
            Game.IsMouseActive = true;
        }

        private void Btn_MouseEntered(object sender, EventArgs e)
        {
            Game.IsMouseActive = false;
        }

        private bool QueueForClosing = false;

        private TimeSpan prev;
        public override void Update(GameTime gameTime)
        {
            if (QueueForClosing)
                Close();

            if (widget.Scale.X < 1)
            {
                if (gameTime.TotalGameTime - prev >= TimeSpan.FromSeconds(1))
                {
                    var plus = 0.005f;
                    widget.Scale = new Vector2(widget.Scale.X + plus, widget.Scale.Y + plus);


                    prev = gameTime.ElapsedGameTime;
                }

                if (prev == default)
                    prev = gameTime.TotalGameTime;
            }

            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }

            var mouse = MouseExtended.GetState();

            // надеемся что оно всегда будет закрываться
            //if (widget.Scale.X >= 1)
            //{
            //    if (mouse.WasButtonPressed(MouseButton.Left) || mouse.WasButtonPressed(MouseButton.Right))
            //    {
            //        QueueForClosing = true;
            //    }
            //}

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (widget.Scale.X >= 1)
            {
                var sb = Game.BeginDraw();
                
                var mouse = MouseExtended.GetState();
                var mousePos = Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

                sb.DrawLine(Game.Camera.ScreenToWorld(_position), mousePos, Color.LightGray, .5f);
            }
        }
    }
}
