using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows
{
    internal class InformationWindow : ScreenWidgetWindow
    {
        private Texture2D _avatar;
        private GameObject _gameObject;
        private FontSystem _font;

        public InformationWindow(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject=gameObject;
        }

        public override bool IsModal => true;

        protected override void LoadContent()
        {
            if (_gameObject.Portrait.IsNotEmpty())
                _avatar = Content.Load<Texture2D>(_gameObject.Portrait);
            else if (_gameObject.Image.IsNotEmpty())
                _avatar = Content.Load<Texture2D>(_gameObject.Image);
            else if (_gameObject.ObjectType== Struct.ObjectType.Ground)
            {
                var groundType = _gameObject.GetPropertyValue<GroundType>(nameof(GroundType));
                var imageAssetPath = Game.DataBase.GetFromDictionary<string>("Data/Objects/GroundTypeMapImages.json", groundType + Game.GameState.LoadedMapPostFix);
                _avatar = Content.Load<Texture2D>(imageAssetPath);
            }

            _font = Content.LoadFont(Fonts.Retron);

            base.LoadContent();
        }

        protected override Window CreateWindow()
        {
            var window = new Window();
            window.Height = 450;
            window.Width = 550;

            var imagePanel = new Panel();
            imagePanel.HorizontalAlignment = HorizontalAlignment.Center;

            if (_avatar != null)
            {
                var avatar = new Image()
                {
                    Renderable = new TextureRegion(_avatar),
                    Width = 167,
                    Height = 250
                };
                avatar.HorizontalAlignment = HorizontalAlignment.Center;

                imagePanel.Widgets.Add(avatar);
            }
            Grid.SetColumn(imagePanel, 0);


            var scroll = new ScrollViewer();
            Grid.SetColumn(scroll, 1);

            var informationpanel = new VerticalStackPanel();
            scroll.Content = informationpanel;
            informationpanel.Padding = new Thickness(0, 5, 0, 0);

            var y = 0;

            var fontSize = 20;

            if (_gameObject.Battler != null) 
            {
                var battler = _gameObject.Battler;
                var wounds = battler.BattlerWounds();
                var hp = new Label()
                {
                    Font = _font.GetFont(fontSize),
                    Text = wounds.WoundName(),
                    TextColor = wounds.WoundColor()
                };
                hp.HorizontalAlignment = HorizontalAlignment.Left;

                var rep = new Label()
                {
                    Font = _font.GetFont(fontSize),
                    Text = _gameObject.Reputation.Name(),
                    TextColor = _gameObject.Reputation.Color(),
                    //Top = fontSize
                };
                rep.HorizontalAlignment = HorizontalAlignment.Left;

                var rating = new Label()
                {
                    Font = _font.GetFont(fontSize),
                    Text =  Game.Strings["GameTexts"][nameof(DangerRating)]+" : "+Game.Strings["Enums/DangerRating"][_gameObject.DangerRating.ToString()],
                    TextColor = Game.DataBase.GetFromDictionary<string>("Data/Enums/DangerRatingColors.json",_gameObject.DangerRating.ToString()).AsColor()
                };
                rep.HorizontalAlignment = HorizontalAlignment.Left;

                informationpanel.Widgets.Add(hp);
                informationpanel.Widgets.Add(rep);
                informationpanel.Widgets.Add(rating);

                y = fontSize;
            }

            var description = new Label()
            {
                Font = _font.GetFont(fontSize),
                Text = Game.Strings.GetObjectDescription(_gameObject),
                Wrap=true,
                Top = y
            };

            informationpanel.Widgets.Add(description);

            var btn = new Button()
            {
                Width = 200,
                Height = 50,
                Background = WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var newgametext = new Label()
            {
                Text = Game.Strings["UI"]["Close"],
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
                Font = _font.GetFont(28),
            };
            btn.Click += (s,e)=>this.Close();
            btn.Content = newgametext;
            btn.PressedBackground = new SolidBrush(Color.Black);

            Grid.SetRow(btn, 1);
            Grid.SetColumnSpan(btn, 2);


            var grid = new Grid();

            grid.ColumnSpacing = 10;
            grid.Padding = new Myra.Graphics2D.Thickness(10);

            grid.Widgets.Add(imagePanel);
            grid.Widgets.Add(scroll);
            grid.Widgets.Add(btn);

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 5));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));

            window.Content = grid;

            window.Padding=Thickness.Zero;

            return window;
        }

        protected override void InitWindow(Window window)
        {
            window.Title = _gameObject.GetObjectName();
            window.TitleFont = _font.GetFont(24);

            window.TitlePanel.Background = this.WindowBackground.NinePatch();
            window.TitlePanel.Padding = Thickness.Zero;

            var label = window.TitlePanel.GetChildren().FirstOrDefault(x => x.GetType() == typeof(Label)).As<Label>();
            if (label != null)
                label.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public static void Open(NabunassarGame game, GameObject gameObject)
        {
            var wind = new InformationWindow(game, gameObject);
            Open(wind);
        }
    }
}
