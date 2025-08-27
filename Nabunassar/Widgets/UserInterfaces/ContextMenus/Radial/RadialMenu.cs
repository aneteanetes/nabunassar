using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using Nabunassar.Systems;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial
{
    internal class RadialMenu : ScreenWidget
    {
        const int PanelWidthHeight = 320;
        const int CircleWidthHeight = 64;

        private static Dictionary<string, TextureRegion> ActionIcons = new();
        private static Texture2D CircleTexture;
        private static Texture2D CircleTexture32;
        private static Texture2D MainTileset;
        private static Texture2D CircleTextureFocused;
        private static Texture2D transparentLineTexture;
        private static Texture2D CursorTileset;
        private static Texture2D TrapsTileset;
        private static TextureRegion MoreActionsArrow;

        private static Color _baseColor = "#cfc6b8".AsColor();

        private Panel _widget;

        protected override bool IsMouseMovementAvailableWithThisActivedWidget => false;

        public GameObject GameObject {  get; private set; }

        private Texture2D _gameObjectImage;

        public RadialMenu(NabunassarGame game, GameObject gameObject, Vector2 position) : base(game)
        {
            GameObject = gameObject;
            Position = position;
        }

        public static void Open(NabunassarGame game, GameObject gameObject, Vector2 position)
        {
            game.AddDesktopWidget(new RadialMenu(game, gameObject, position));
            game.GameState.Cursor.SetCursor("cursor");
        }

        public override void LoadContent()
        {
            if (CircleTexture == default)
            {
                CircleTexture32 = Content.Load<Texture2D>("Assets/Images/Interface/Circle32_f.png");
                CircleTexture = Content.Load<Texture2D>("Assets/Images/Interface/Circle64.png");
                CircleTextureFocused = Content.Load<Texture2D>("Assets/Images/Interface/Circle64_f.png");
                MainTileset = Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");

                transparentLineTexture = Content.Load<Texture2D>("Assets/Images/Interface/TransparentLine.png");

                CursorTileset = Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png");
                TrapsTileset = Content.Load<Texture2D>("Assets/Tilesets/traps.png");

                ActionIcons["info"] = new TextureRegion(CursorTileset, new Rectangle(192, 32, 16, 16));
                ActionIcons["speak"] = new TextureRegion(CursorTileset, new Rectangle(160, 32, 16, 16));
                ActionIcons["moveto"] = new TextureRegion(CursorTileset, new Rectangle(288, 48, 16, 16));
                ActionIcons["spell"] = new TextureRegion(MainTileset, new Rectangle(448, 176, 16, 16));
                ActionIcons["attack"] = new TextureRegion(MainTileset, new Rectangle(544, 96, 16, 16));
                ActionIcons["skill"] = new TextureRegion(MainTileset, new Rectangle(464, 192, 16, 16));
                ActionIcons["steal"] = new TextureRegion(MainTileset, new Rectangle(672, 64, 16, 16));
                ActionIcons["lockpick"] = new TextureRegion(CursorTileset, new Rectangle(304, 16, 16, 16));
                ActionIcons["enter"] = new TextureRegion(CursorTileset, new Rectangle(256, 16, 16, 16));
                ActionIcons["trap"] = new TextureRegion(MainTileset, new Rectangle(432, 192, 16, 16));
                ActionIcons["open"] = new TextureRegion(MainTileset, new Rectangle(704, 144, 16, 16));

                ActionIcons["trapsearch"] = new TextureRegion(TrapsTileset, new Rectangle(32, 0, 16, 16));
                ActionIcons["disarmtrap"] = new TextureRegion(TrapsTileset, new Rectangle(0, 0, 16, 16));
                ActionIcons["taketrap"] = new TextureRegion(TrapsTileset, new Rectangle(16, 0, 16, 16));

                MoreActionsArrow = new TextureRegion(CursorTileset,new Rectangle(160,64,16, 16));
            }

            if (GameObject != default && GameObject.Image != default)
                _gameObjectImage = Content.Load<Texture2D>(GameObject.Image);
        }

        protected override void UnloadContent()
        {
            _gameObjectImage = null;
            base.UnloadContent();
        }

        private Panel _globalPanel;

        protected override Widget CreateWidget()
        {
            Game.DisableMouseSystems();

            _globalPanel = new Panel();

            Fullfill(null,null,GameObject);

            _globalPanel.TouchDown += GlobalPanel_TouchDown;

            return _globalPanel;
        }

        private void GlobalPanel_TouchDown(object sender, EventArgs e)
        {
            QueueForClosing = true;
        }

        private void FillActionsBasedOnObjectType(Panel panel)
        {
            FillBaseActions(panel);

            if (GameObject == null)
            {
                FillEmptyActions(panel);
                return;
            }

            FillInfoAction(panel);

            switch (GameObject.ObjectType)
            {
                case ObjectType.Ground:
                    FillEmptyActions(panel);
                    break;
                case ObjectType.NPC:
                    FillNPCActions(panel);
                    break;
                case ObjectType.Door:
                    FillDoorActions(panel);
                    break;
                case ObjectType.Container:
                    FillContainerActions(panel);
                    break;
                default:
                    break;
            }
        }

        public void Fullfill(IEnumerable<RadialMenuAction> actions=null, RadialMenuAction prevAction=null, GameObject gameObject=null)
        {
            Mouse.SetPosition((int)Position.X, (int)Position.Y);
            _globalPanel.Widgets.Clear();

            var panel = _widget = new Panel();
            panel.Width = PanelWidthHeight;
            panel.Height = PanelWidthHeight;

            _globalPanel.Widgets.Add(panel);

            panel.Top = (int)Math.Floor(Position.Y - PanelWidthHeight / 2);
            panel.Left = (int)Math.Floor(Position.X - PanelWidthHeight / 2);

            TextureRegion imageRegion = default;
            if (_gameObjectImage != null)
            {
                imageRegion = new TextureRegion(_gameObjectImage, new Rectangle(0, 0, _gameObjectImage.Width, _gameObjectImage.Height));

                var centerImage = new Image()
                {
                    Renderable = imageRegion,
                    Width = 140,
                    Height = 140
                };
                centerImage.HorizontalAlignment = HorizontalAlignment.Center;
                centerImage.VerticalAlignment = VerticalAlignment.Center;
                panel.Widgets.Add(centerImage);
            }

            if (prevAction != null)
            {
                var (btn, content) = CreateIconButton(panel, prevAction.CreateBackAction(), new RadialActionPosition()
                {
                    X = PanelWidthHeight / 2 - CircleWidthHeight / 2,
                    Y = PanelWidthHeight / 2 - CircleWidthHeight / 2
                }, new List<Panel>());
            }

            if (actions == null)
            {
                FillActionsBasedOnObjectType(panel);
            }
            else
            {
                foreach (var action in actions)
                {
                    AddAction(panel, action);
                }
            }
        }

        private void FillInfoAction(Panel panel)
        {
            AddAction(panel, new InformationRadialAction(this));
        }

        private void FillDoorActions(Panel panel)
        {
            if (GameObject.GetPropertyValue<bool>("IsLocked"))
                AddAction(panel, new LockpickRadialAction(this));

            AddAction(panel, new EnterDoorRadialAction(this));
            AddAction(panel, new TrapsRadialAction(this));
        }

        private void FillContainerActions(Panel panel)
        {
            if (GameObject.GetPropertyValue<bool>("IsLocked"))
                AddAction(panel, new LockpickRadialAction(this));

            AddAction(panel, new AttackRadialAction(this));
            AddAction(panel, new TrapsRadialAction(this));
            AddAction(panel, new OpenRadialAction(this));
        }

        private void FillNPCActions(Panel panel)
        {
            AddAction(panel, new SpeakToRadialAction(this));
            AddAction(panel, new AttackRadialAction(this));
            AddAction(panel, new StealRadialAction(this));
        }

        private void FillBaseActions(Panel panel)
        {
            AddAction(panel, new SpellsRadialAction(this));

            List<SkillAbilityRadialAction> skillActions = new();

            SkillAbilityRadialAction.ResetCounters();

            foreach (var abil in Game.GameState.Party.GetWorldAbilities(GameObject))
            {
                var abilIconTexture = Content.Load<Texture2D>(abil.Icon);

                var isEnable = abil.IsActive(GameObject);

                var iconName = abil.Name;
                if(!isEnable && isEnable.Message.IsNotEmpty())
                    iconName = isEnable.Message;

                skillActions.Add(new SkillAbilityRadialAction(this, abil, iconName, new TextureRegion(abilIconTexture))
                {
                    IsEnabled = isEnable
                });
            }

            AddAction(panel, new SkillRadialAction(this, skillActions));
        }

        private void FillEmptyActions(Panel panel)
        {
            AddAction(panel, new MoveToRadialAction(this));
        }

        private void AddAction(Panel panel, RadialMenuAction action)
        {
            var actionPos = SetActionPosition(action);

            if (actionPos.IsDraw)
            {
                List<Panel> innerActionsWidgets = new();

                var (btn,btnContent) = CreateIconButton(panel, action, actionPos, innerActionsWidgets);

                panel.Scale = Vector2.One * .001f;

                if (action.InnerActions.IsNotEmpty())
                {
                    var ratio = 2;

                    var moreArrowPosition = SetMoreArrowPosition(action.Position);
                    var moreArrow = new Image() { Renderable = MoreActionsArrow };
                    moreArrow.Left = moreArrowPosition.X;
                    moreArrow.Top = moreArrowPosition.Y;
                    moreArrow.Rotation = moreArrowPosition.Rotation;

                    btnContent.Widgets.Add(moreArrow);

                    foreach (var innerAction in action.InnerActions)
                    {
                        var innerActionPos = SetInnerActionPosition(innerAction);

                        var innerContainer = new Panel();
                        innerContainer.Visible = false;
                        innerContainer.Width = CircleWidthHeight / ratio;
                        innerContainer.Height = innerContainer.Width;
                        innerContainer.HorizontalAlignment = HorizontalAlignment.Center;
                        innerContainer.VerticalAlignment = VerticalAlignment.Center;

                        innerContainer.Left = innerActionPos.X;
                        innerContainer.Top = innerActionPos.Y;

                        var innerCircle = new Image()
                        {
                            Renderable = new TextureRegion(CircleTexture32, new Rectangle(0, 0, 32, 32))
                        };
                        innerCircle.HorizontalAlignment = HorizontalAlignment.Center;
                        innerCircle.VerticalAlignment = VerticalAlignment.Center;
                        innerCircle.Width = CircleWidthHeight / ratio;
                        innerCircle.Height = CircleWidthHeight / ratio;

                        //innerCircle.Left = innerActionPos.X;
                        //innerCircle.Top = innerActionPos.Y;

                        innerContainer.Widgets.Add(innerCircle);

                        var innerIcon = new Image()
                        {
                            Renderable = innerAction.Icon ?? ActionIcons[innerAction.CodeName]
                        };
                        innerIcon.Color = _baseColor;
                        innerIcon.Width = 24;
                        innerIcon.Height = 24;
                        innerIcon.HorizontalAlignment = HorizontalAlignment.Center;
                        innerIcon.VerticalAlignment = VerticalAlignment.Center;

                        if (!innerAction.IsEnabled)
                        {
                            innerIcon.Color = Color.Gray;
                        }

                        //innerIcon.Left = innerActionPos.X;
                        //innerIcon.Top = innerActionPos.Y;

                        innerContainer.Widgets.Add(innerIcon);

                        btnContent.Widgets.Add(innerContainer);
                        innerActionsWidgets.Add(innerContainer);
                    }
                }
            }
        }

        private (Button, Panel) CreateIconButton(Panel panel, RadialMenuAction action, RadialActionPosition actionPos, List<Panel> innerActionsWidgets)
        {
            var btn = new Button()
            {
                Background = new TextureRegion(CircleTexture, new Rectangle(0, 0, CircleWidthHeight, CircleWidthHeight)),
                Width = CircleWidthHeight,
                Height = CircleWidthHeight
            };

            var token = action.Is<BackRadialAction>() ? "back" : action.CodeName;

            var title = action.Name ?? Game.Strings["UI"][token];
            var iconTexture = action.Icon ?? ActionIcons[action.CodeName];

            btn.MouseEntered += (sender, @event) =>
            {
                if (innerActionsWidgets.Count > 0)
                {
                    innerActionsWidgets.ForEach(iactw => iactw.Visible = true);
                }
                Btn_MouseEntered(sender, @event, title,action.IsEnabled);
            };
            btn.MouseLeft += (sender, @event) =>
            {
                if (innerActionsWidgets.Count > 0)
                {
                    innerActionsWidgets.ForEach(iactw => iactw.Visible = false);
                }
                Btn_MouseLeft(sender, @event);
            };
            btn.TouchDown += (s, e) => QueueForClosing = false; //здесь можно отменить закрытие по клику
            btn.Click += (s, e) => action.OnClick(); // а здесь обрабатывается событие по клику на кнопке
            btn.FocusedBackground = new TextureRegion(CircleTextureFocused, new Rectangle(0, 0, CircleWidthHeight, CircleWidthHeight));
            btn.OverBackground = btn.FocusedBackground;
            btn.PressedBackground = btn.OverBackground;

            btn.Left = actionPos.X;
            btn.Top = actionPos.Y;

            var icon = new Image()
            {
                Renderable = iconTexture
            };
            icon.Color = _baseColor;
            icon.Width = CircleWidthHeight - CircleWidthHeight / 4;
            icon.Height = CircleWidthHeight - CircleWidthHeight / 4;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;

            var btnContent = new Panel();
            btnContent.HorizontalAlignment = HorizontalAlignment.Center;
            btnContent.VerticalAlignment = VerticalAlignment.Center;
            btnContent.Width = btn.Width;
            btnContent.Height = btn.Height;
            btnContent.Widgets.Add(icon);

            if (!action.IsEnabled)
            {
                btn.Enabled = false;
                btn.Background = btn.OverBackground;
                icon.Color = Color.Gray;
            }

            btn.Content = btnContent;
            panel.Widgets.Add(btn);

            btn.BringToFront();


            return (btn,btnContent);
        }

        private RadialActionPosition SetMoreArrowPosition(Direction direction)
        {
            var x = 0;
            var y = 0;
            var rotation = 0f;

            var draw = true;

            switch (direction)
            {
                case Direction.Up:
                    y = -16;
                    x = 24;
                    break;
                case Direction.Down:
                    y = 64;
                    x = 24;
                    rotation = 180;
                    break;
                case Direction.Left:
                    y = 24;
                    x = -16;
                    rotation = 270;
                    break;
                case Direction.Right:
                    x = 64;
                    y = 24;
                    rotation = 90;
                    break;
                case Direction.UpLeft:
                    y = -8;
                    x = -8;
                    rotation = 315;
                    break;
                case Direction.UpRight:
                    x = 54;
                    y = -8;
                    rotation = 45;
                    break;
                case Direction.DownLeft:
                    y = 54;
                    x = -8;
                    rotation = 225;
                    break;
                case Direction.DownRight:
                    y = 54;
                    x = 54;
                    rotation = 140;
                    break;
                default:
                    draw = false;
                    break;
            }

            return new RadialActionPosition()
            {
                X = x,
                Y = y,
                IsDraw = draw,
                Rotation = rotation
            };
        }

        private RadialActionPosition SetInnerActionPosition(RadialMenuAction action)
        {
            var x = 0;
            var y = 0;

            var draw = true;

            switch (action.Position)
            {
                case Direction.Up:
                    y = -32;
                    break;
                case Direction.Down:
                    y = 32;
                    break;
                case Direction.Left:
                    x = -32;
                    break;
                case Direction.Right:
                    x = 32;
                    break;
                case Direction.UpLeft:
                    y = -32;
                    x = -32;
                    break;
                case Direction.UpRight:
                    y = -32;
                    x = 32;
                    break;
                case Direction.DownLeft:
                    y = 32;
                    x = -32;
                    break;
                case Direction.DownRight:
                    y = 32;
                    x = 32;
                    break;
                default:
                    draw = false;
                    break;
            }

            return new RadialActionPosition()
            {
                X = x,
                Y = y,
                IsDraw = draw
            };
        }

        private RadialActionPosition SetActionPosition(RadialMenuAction action)
        {
            var centerCircleRadius = PanelWidthHeight;
            var actionCircleRadius = CircleWidthHeight;

            var x = 0;
            var y = 0;
            void middleX()
            {
                x = centerCircleRadius / 2 - actionCircleRadius / 2;
            }

            void downY()
            {
                y = centerCircleRadius - actionCircleRadius;
            }

            void middleY()
            {
                y = actionCircleRadius * 2;
            }

            void upHalfPartY()
            {
                y = actionCircleRadius / 2 + actionCircleRadius / 4;
            }

            void downHalfPartY()
            {
                upHalfPartY();
                y += actionCircleRadius * 2;
                y += actionCircleRadius / 2;
            }

            var xoffset = actionCircleRadius + actionCircleRadius / 4;

            void shiftX(bool isPlus = true)
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

            var fourthPart = actionCircleRadius / 4;
            var draw = true;

            switch (action.Position)
            {
                case Direction.Up:
                    middleX();
                    y += fourthPart;
                    break;
                case Direction.Down:
                    middleX(); downY();
                    y -= fourthPart;
                    break;
                case Direction.Left:
                    middleY();
                    x += fourthPart;
                    break;
                case Direction.Right:
                    middleY(); x = centerCircleRadius - actionCircleRadius;
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

            return new RadialActionPosition()
            {
                X = x,
                Y = y,
                IsDraw = draw
            };
        }

        private TitleWidget _titleWidget;

        private void Btn_MouseLeft(object sender, EventArgs e)
        {
            var btn = sender.As<Button>();
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        private void Btn_MouseEntered(object sender, EventArgs e, string text, bool isEnabled=true)
        {
            var btn = sender.As<Button>();
            var parent = btn.Parent;

            var left = btn.Left + parent.Left;
            var top = btn.Top + parent.Top;


            _titleWidget = new TitleWidget(Game, text, new Vector2(left, top), isEnabled ? Color.White : Color.Gray);
            Game.AddDesktopWidget(_titleWidget);
        }

        private bool QueueForClosing = false;

        public override void Close()
        {
            Game.EnableSystems();
            PlayerControllSystem.IsPreventNextMove = true;
            base.Close();
        }

        private TimeSpan prev;
        public override void Update(GameTime gameTime)
        {
            if (QueueForClosing)
                Close();

            if (_widget.Scale.X < 1)
            {
                if (gameTime.TotalGameTime - prev >= TimeSpan.FromSeconds(1))
                {
                    var plus = 0.005f;
                    _widget.Scale = new Vector2(_widget.Scale.X + plus, _widget.Scale.Y + plus);


                    prev = gameTime.ElapsedGameTime;
                }

                if (prev == default)
                    prev = gameTime.TotalGameTime;
            }

            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Keys.Escape))
            {
                Close();
            }

            var mouse = MouseExtended.GetState();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_widget.Scale.X >= 1)
            {
                var sb = Game.BeginDraw(false, samplerState: SamplerState.LinearClamp);

                var mouse = MouseExtended.GetState();
                var toMousePos = mouse.Position.ToVector2();//Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

                toMousePos += new Vector2(1.5f, 0);

                var fromCenterPos = Position;/*Game.Camera.ScreenToWorld(_position);*/

                float length = Vector2.Distance(fromCenterPos, toMousePos);
                float angle = (float)Math.Atan2(toMousePos.Y - fromCenterPos.Y, toMousePos.X - fromCenterPos.X);

                sb.Draw(transparentLineTexture, new Rectangle((int)fromCenterPos.X, (int)fromCenterPos.Y, (int)length, 5), new Rectangle(0, 0, transparentLineTexture.Width, 1), Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
            }
        }
    }
}
