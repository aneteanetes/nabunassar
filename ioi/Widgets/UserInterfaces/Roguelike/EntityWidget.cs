using FontStashSharp;
using ioi.Components;
using ioi.Struct;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class EntityWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;

        private GameEntity entity;
        private Side side;
        private Label name;
        private Label raceclass;
        private Label level;
        private Label exp;
        private Label health;
        private Label resource;
        private Label damage;
        private Label ad;
        private Label ap;
        private Label def;
        private Label mdef;
        private Label gold;
        private Label squad;

        public EntityWidget(GameHost game, GameEntity entity, Side side) : base(game)
        {
            this.entity = entity;

            this.side = side;

            Position = side == Side.Right
                ? new Vector2(1488, 24)
                : new Vector2(14, 24);
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
        }

        protected override Widget CreateWidget()
        {
            var panel = new VerticalStackPanel
            {
                Width = 415,
                Height = 800
            };

            var strings = Game.Strings["Roguelike"];

            var largeBottomMargin = 20;

            name = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0,15,10,largeBottomMargin),
                TextColor = Color.Cyan,
                Font = consolas
            };

            raceclass = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin),
                TextColor = new Color(204,202,204),
                Font = consolas
            };

            level = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(118, 118, 102),
                Font = consolas
            };

            exp = new Label
            {                
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 5, 10, 0),
                TextColor = new Color(118, 118, 102),
                Font = consolas
            };

            health = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10+ largeBottomMargin, 10, 0),
                TextColor = new Color(231, 72, 75),
                Font = consolas
            };

            resource = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin/2),
                TextColor = entity.Color("rescolor"),
                Font = consolas
            };

            damage = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(193,156,0,255),
                Font = consolas
            };

            ad = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(197, 15, 31, 255),
                Font = consolas
            };

            ap = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(58, 150, 221, 255),
                Font = consolas
            };

            def = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 0),
                TextColor = new Color(19, 161, 14, 255),
                Font = consolas
            };

            mdef = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, largeBottomMargin),
                TextColor = new Color(136, 23, 152, 255),
                Font = consolas
            };

            gold = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 50),
                TextColor = Color.Gold,
                Font = consolas
            };

            var squadcellsize = 60;

            var effectsGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding=new Myra.Graphics2D.Thickness(4,0,0,0),
                Margin=new Myra.Graphics2D.Thickness(0,5)
            };

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    var cell = new Label
                    {
                        Text = "",
                        TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                        //Padding = new Myra.Graphics2D.Thickness(0, 15),
                        //Margin = new Myra.Graphics2D.Thickness(5),
                        TextColor = Color.LightSeaGreen,
                        Font = consolas,
                        Height = squadcellsize/2,
                        Width = squadcellsize/2
                    };
                    effectsGrid.Widgets.Add(cell);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                }
            }

            effectsGrid.Width = squadcellsize * 6 + 30;

            squad = new Label
            {
                Text = $"{strings["squad"]}:",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 5),
                TextColor = Color.IndianRed,
                Font = consolas
            };

            var squadgrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                //Border = new SolidBrush(Color.IndianRed),
                //BorderThickness = new Myra.Graphics2D.Thickness(1)
            };
            var c = 0;
            foreach (var member in entity.Squad)
            {
                var cell = new Label
                {
                    Text = member["icon"].String,
                    TextColor = member.Color("color"),
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Myra.Graphics2D.Thickness(0, 10),
                    Margin = new Myra.Graphics2D.Thickness(5),
                    Font = consolas,
                    Border = new SolidBrush(entity == member ? Color.IndianRed : Color.DarkGoldenrod),
                    BorderThickness = new Myra.Graphics2D.Thickness(1),
                    Height = squadcellsize,
                    Width = squadcellsize
                };

                squadgrid.Widgets.Add(cell);
                Grid.SetColumn(cell, c);

                c++;
            }

            squadgrid.Width = squadcellsize * 6 + 30;

            panel.Widgets.Add(name);
            panel.Widgets.Add(raceclass);
            panel.Widgets.Add(level);

            if (side == Side.Right)
                panel.Widgets.Add(exp);

            panel.Widgets.Add(health);

            if (side == Side.Right)
                panel.Widgets.Add(resource);
            
            panel.Widgets.Add(effectsGrid);
            panel.Widgets.Add(damage);
            panel.Widgets.Add(ad);
            panel.Widgets.Add(ap);
            panel.Widgets.Add(def);
            panel.Widgets.Add(mdef);
            panel.Widgets.Add(gold);
            panel.Widgets.Add(squad);
            panel.Widgets.Add(squadgrid);

            return panel;
        }

        public override void Update(GameTime gameTime)
        {
            var strings = Game.Strings["Roguelike"];

            name.Text = entity.Name ?? strings[entity["name"].String];
            raceclass.Text = $"{strings[entity["race"].String]} - {strings[entity["class"].String]}";
            level.Text = $"{strings["level"]}: {entity["level"]}";
            exp.Text = $"{strings["exp"]}: {entity["exp"]}/10";
            health.Text = $"{strings["health"]}: {entity["stats.hp"]}/{entity["stats.mhp"]}";
            resource.Text = $"{strings[entity["res"].String]}: {entity.Func("resstring").String}";
            damage.Text = $"{strings["damage"]}: {entity["stats.mindmg"]}-{entity["stats.maxdmg"]}";
            ad.Text = $"{strings["ad"]}: {entity["stats.ad"]}";
            ap.Text = $"{strings["ap"]}: {entity["stats.ap"]}";
            def.Text = $"{strings["def"]}: {entity["stats.def"]}";
            mdef.Text = $"{strings["mdef"]}: {entity["stats.mdef"]}";
            gold.Text = $"{strings["gold"]}: {entity["gold"]}";
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }
    }
}
