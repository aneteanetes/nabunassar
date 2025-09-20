using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Struct;
using Nabunassar.Extensions.LocalizedStringsExtensions;
using Nabunassar.Struct;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Informations
{
    internal class InformationWindow : ScreenWidgetWindow
    {
        private Texture2D _avatar;
        protected GameObject GameObject;
        protected FontSystem Font;

        public InformationWindow(NabunassarGame game, GameObject gameObject) : base(game)
        {
            GameObject = gameObject;
        }

        public override bool IsModal => true;

        public override void LoadContent()
        {
            if (Portrait.IsEmpty())
            {
                if (GameObject.Portrait.IsNotEmpty())
                    _avatar = Content.Load<Texture2D>(GameObject.Portrait);
                else if (GameObject.Image.IsNotEmpty())
                    _avatar = Content.Load<Texture2D>(GameObject.Image);
                else if (GameObject.ObjectType == ObjectType.Ground)
                {
                    var groundType = GameObject.GetPropertyValue<GroundType>(nameof(GroundType));
                    var imageAssetPath = Game.DataBase.GetFromDictionary<string>("Data/Objects/GroundTypeMapImages.json", groundType + Game.GameState.LoadedMapPostFix);
                    _avatar = Content.Load<Texture2D>(imageAssetPath);
                }
            }
            else
            {
                _avatar = Content.Load<Texture2D>(Portrait);
            }

            Font = Content.LoadFont(Fonts.Retron);

            base.LoadContent();
        }

        protected virtual string Portrait => default;

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

            var fontSize = 20;

            var topOffset = FillInformationWindow(informationpanel);

            var desc = Game.Strings.GetObjectDescription(GameObject);
            if (desc.IsFound())
            {
                var description = new Label()
                {
                    Font = Font.GetFont(fontSize),
                    Text = Game.Strings.GetObjectDescription(GameObject),
                    Wrap = true,
                    Top = topOffset
                };
                informationpanel.Widgets.Add(description);
            }

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
                Font = Font.GetFont(28),
            };
            btn.Click += (s, e) => Close();
            btn.Content = newgametext;
            btn.PressedBackground = new SolidBrush(Color.Black);

            Grid.SetRow(btn, 1);
            Grid.SetColumnSpan(btn, 2);


            var grid = new Grid();

            grid.ColumnSpacing = 10;
            grid.Padding = new Thickness(10);

            grid.Widgets.Add(imagePanel);
            grid.Widgets.Add(scroll);
            grid.Widgets.Add(btn);

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 5));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));

            window.Content = grid;

            window.Padding = Thickness.Zero;

            return window;
        }

        protected virtual int FillInformationWindow(VerticalStackPanel informationpanel)
        {
            var fontSize = 20;

            if (GameObject.Battler != null)
            {
                var battler = GameObject.Battler;
                var wounds = battler.BattlerWounds();
                var hp = new Label()
                {
                    Font = Font.GetFont(fontSize),
                    Text = wounds.WoundName(),
                    TextColor = wounds.WoundColor()
                };
                hp.HorizontalAlignment = HorizontalAlignment.Left;

                var rep = new Label()
                {
                    Font = Font.GetFont(fontSize),
                    Text = DrawText.Create(Game.Strings["GameTexts"]["Repuration relations"]+" : ").Color(GameObject.Reputation.Color()).Append(GameObject.Reputation.Name()).ToString(),
                    //Top = fontSize
                };
                rep.HorizontalAlignment = HorizontalAlignment.Left;

                var rating = new Label()
                {
                    Font = Font.GetFont(fontSize),
                    Text = Game.Strings["GameTexts"][nameof(DangerRating)] + " : " + Game.Strings["Enums/DangerRating"][GameObject.DangerRating.ToString()],
                    TextColor = Game.DataBase.GetFromDictionary<string>("Data/Enums/DangerRatingColors.json", GameObject.DangerRating.ToString()).AsColor()
                };
                rep.HorizontalAlignment = HorizontalAlignment.Left;

                informationpanel.Widgets.Add(hp);
                informationpanel.Widgets.Add(rep);
                informationpanel.Widgets.Add(rating);

                return fontSize;
            }

            return 0;
        }

        protected override void InitWindow(Window window)
        {
            StandartWindowTitle(window, GameObject.GetObjectName());
        }

        public static void Open(NabunassarGame game, GameObject gameObject)
        {
            var wind = new InformationWindow(game, gameObject);
            Open(wind);
        }
    }
}
