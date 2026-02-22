using FontStashSharp;
using ioi.Components;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class PlayerWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;

        public PlayerWidget(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
            Position = new Vector2(1488, 25);
        }

        protected override Widget CreateWidget()
        {
            var panel = new VerticalStackPanel
            {
                Width = 415,
                Height = 800
            };

            var player = Game.GameState.Player;
            var strings = Game.Strings["Roguelike"];

            var largeBottomMargin = 20;

            var name = new Label
            {
                Text = player.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0,15,10,largeBottomMargin),
                TextColor = Color.Cyan,
                Font = consolas
            };

            var raceclass = new Label
            {
                Text = $"{strings[player["race"].String]} - {strings[player["class"].String]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin),
                TextColor = new Color(204,202,204),
                Font = consolas
            };

            var level = new Label
            {
                Text = $"{strings["level"]}: {player["level"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(118, 118, 102),
                Font = consolas
            };

            var exp = new Label
            {
                Text = $"{strings["exp"]}: {player["exp"]}/10",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 5, 10, largeBottomMargin),
                TextColor = new Color(118, 118, 102),
                Font = consolas
            };

            var health = new Label
            {
                Text = $"{strings["health"]}: {player["stats.hp"]}/{player["stats.mhp"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(231, 72, 75),
                Font = consolas
            };

            var resource = new Label
            {
                Text = $"{strings[player["res"].String]}: {player.Func("resstring").String}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin),
                TextColor = player.Color("rescolor"),
                Font = consolas
            };

            var damage = new Label
            {
                Text = $"{strings["damage"]}: {player["stats.mindmg"]}-{player["stats.maxdmg"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(193,156,0,255),
                Font = consolas
            };

            var ad = new Label
            {
                Text = $"{strings["ad"]}: {player["stats.ad"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(197, 15, 31, 255),
                Font = consolas
            };

            var ap = new Label
            {
                Text = $"{strings["ap"]}: {player["stats.ap"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(58, 150, 221, 255),
                Font = consolas
            };

            var def = new Label
            {
                Text = $"{strings["def"]}: {player["stats.def"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(19, 161, 14, 255),
                Font = consolas
            };

            var mdef = new Label
            {
                Text = $"{strings["mdef"]}: {player["stats.mdef"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin),
                TextColor = new Color(136, 23, 152, 255),
                Font = consolas
            };

            var gold = new Label
            {
                Text = $"{strings["gold"]}: {player["gold"]}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 30),
                TextColor = Color.Gold,
                Font = consolas
            };
            
            var squad = new Label
            {
                Text = $"{strings["squad"]}:",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 5),
                TextColor = Color.IndianRed,
                Font = consolas
            };

            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Border = new SolidBrush(Color.IndianRed),
                BorderThickness = new Myra.Graphics2D.Thickness(1)
            };

            var cellsize = 60;
            var counter = 1;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Label
                    {
                        Text = counter.ToString(),
                        TextAlign= FontStashSharp.RichText.TextHorizontalAlignment.Center,
                        Padding=new Myra.Graphics2D.Thickness(0,20),
                        TextColor = Color.IndianRed,
                        Font = consolas,
                        Border = new SolidBrush(Color.IndianRed),
                        BorderThickness = new Myra.Graphics2D.Thickness(1),
                        Height= cellsize,
                        Width= cellsize
                    };
                    counter++;
                    grid.Widgets.Add(cell);
                    Grid.SetRow(cell,i);
                    Grid.SetColumn(cell,j);
                }
            }
            grid.Width = cellsize * 3;
            grid.Height = cellsize * 2;

            panel.Widgets.Add(name);
            panel.Widgets.Add(raceclass);
            panel.Widgets.Add(level);
            panel.Widgets.Add(exp);
            panel.Widgets.Add(health);
            panel.Widgets.Add(resource);
            panel.Widgets.Add(damage);
            panel.Widgets.Add(ad);
            panel.Widgets.Add(ap);
            panel.Widgets.Add(def);
            panel.Widgets.Add(mdef);
            panel.Widgets.Add(gold);
            panel.Widgets.Add(squad);
            panel.Widgets.Add(grid);

            return panel;
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }
    }
}
