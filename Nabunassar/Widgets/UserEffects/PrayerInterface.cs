using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Resources;
using Nabunassar.Shaders.Blooming;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserEffects
{
    internal class PrayerInterface : ScreenWidget
    {
        private PrayerAbility _prayerAbility;
        private List<GodImage> godsImages = new();
        private Dictionary<int, Point> positions = new()
        {
            {1,new Point(1150,250) },
            {2,new Point(1150,425) },
            {3,new Point(1150,600) },
            {4,new Point(1150,775) },
            {5,new Point(950,875) },
            {6,new Point(740,875) },
            {7,new Point(530,775) },
            {8,new Point(530,600) },
            {9,new Point(530,425) },
            {10,new Point(530,250) },
            {11,new Point(740,180) },
            {12,new Point(950,180) },
        };
        Texture2D background;
        private BloomShader _bloom;
        private FontSystem _retron;
        private Label _godName;
        private VerticalStackPanel _description;
        private Gods? _selected;
        private Image _selectedImage;

        public override bool IsModal => true;

        public PrayerInterface(NabunassarGame game, PrayerAbility ability) : base(game)
        {
            _prayerAbility = ability;
        }

        public override void LoadContent()
        {
            _bloom = new BloomShader(Game, true, BloomShader.BloomPresets.Wide);
            _bloom.Enable();

            _retron = Content.LoadFont(Fonts.Retron);

            var gods = typeof(Gods).GetAllValues<Gods>()
                .ToArray();

            background = Content.LoadTexture($"Assets/Images/Icons/Gods/_background.png");

            for (int i = 0; i < 12; i++)
            {
                var god = gods[i];
                Point pos = GetPositionByGod(((int)god));
                var texture = Content.LoadTexture($"Assets/Images/Icons/Gods/{god}.png");
                godsImages.Add(new GodImage(texture, pos));
            }
        }

        private Point GetPositionByGod(int god)
        {
            var pos = positions[god];
            pos.X += 50;
            pos.Y -= 50;

            return pos;
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardExtended.GetState().WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }

            var mouse = MouseExtended.GetState();

            godsImages.ForEach(x => x.Color = Color.White);

            var intersected = godsImages.FirstOrDefault(x => x.Bounds.Intersects(new Rectangle(mouse.Position, new Point(1, 1))));
            if (intersected != null)
            {
                intersected.Color = Color.BlueViolet;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //var sb = Game.BeginDraw(false);

            //foreach (var god in godsImages)
            //{
            //    sb.Draw(background, new Rectangle(god.Bounds.X, god.Bounds.Y, 128, 128), Color.White);
            //    sb.Draw(god.Texture, new Rectangle(god.Bounds.X, god.Bounds.Y, 128, 128), god.Color);
            //}

            //sb.End();
        }

        protected override Widget CreateWidget()
        {
            var panel = new Panel() { Width = Game.Resolution.Width, Height = Game.Resolution.Height };
            panel.Background = new SolidBrush(new Color(Color.Black, 150));

            var descriptionPanel = new Panel()
            {
                Background = ScreenWidgetWindow.WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 400,
                Height = 500,
                DragDirection = DragDirection.None,
            };


            var gods = typeof(Gods).GetAllValues<Gods>()
                .ToArray();

            var imgPanel = new HorizontalStackPanel();
            imgPanel.HorizontalAlignment = HorizontalAlignment.Center;
            imgPanel.VerticalAlignment = VerticalAlignment.Top;
            imgPanel.Top = 250;

            for (int i = 0; i < 12; i++)
            {
                var god = gods[i];
                Point pos = GetPositionByGod(((int)god));
                var texture = Content.LoadTexture($"Assets/Images/Icons/Gods/Runes/{god}.png");

                var img =new Image()
                {
                    Renderable = new TextureRegion(texture),
                    Margin = new Myra.Graphics2D.Thickness(25),
                    Height = 60*2,
                    Width = 54*2,
                };
                img.Height += 25;
                img.Width += 25;
                img.MouseEntered += (s, e) =>
                {
                    Select(img,god);
                };
                img.MouseLeft += (s, e) =>
                {
                    Select(img,null);
                };
                img.TouchDown += (s, e) =>
                {
                    Select(img, god, true);
                };
                imgPanel.Widgets.Add(img);
            }

            var selectText = new Label()
            {
                Text = Game.Strings["GameTexts"]["Select God"] + ":",
                Font = _retron.GetFont(48),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Top = 100,
                TextColor = Globals.BaseColor
            };
            panel.Widgets.Add(selectText);

            _godName = selectText.Clone().As<Label>();
            _godName.Text = "";
            _godName.Top = 165;
            panel.Widgets.Add(_godName);

            _description = new VerticalStackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = 50
            };
            panel.Widgets.Add(_description);

            panel.Widgets.Add(imgPanel);

            return panel;
        }

        private void SetDescription(Gods god)
        {
            //_description
        }

        private void Select(Image img, Gods? god, bool isSelection = false)
        {
            if (god == null)
            {
                if (_selected == default)
                {
                    _godName.Text = "";
                    img.Color = Color.White;
                }
                else
                {
                    img.Color = Color.White;
                    Select(_selectedImage, _selected);
                }
                return;
            }

            if (isSelection)
            {
                _selected = god;

                if (_selectedImage != null)
                    _selectedImage.Color = Color.White;

                _selectedImage = img;
            }

            img.Color = Color.Gray;
            _godName.Text = Game.Strings["GodNames"][god.ToString()];
        }

        public override void Close()
        {
            _bloom.Disable();
            base.Close();
        }

        private class GodImage
        {
            public GodImage(Texture2D texture, Point position)
            {
                Bounds = new Rectangle()
                {
                    Height = 128,
                    Width = 128,
                    X = position.X,
                    Y = position.Y
                };
                Texture = texture;
            }

            public Color Color { get; set; }

            public Texture2D Texture { get; set; }

            public Rectangle Bounds { get; set; }
        }
    }
}