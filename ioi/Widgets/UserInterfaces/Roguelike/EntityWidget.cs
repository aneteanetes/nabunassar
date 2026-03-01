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

        public Func<GameEntity> EntityFetcher { get; private set; }

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
        private EffectsGrid effectsGrid;
        private Label squad;
        private SquadGrid squadgrid;

        public EntityWidget(GameHost game, Func<GameEntity> entityFetcher, Side side, GameEntity staticEntity=null) : base(game)
        {
            this.EntityFetcher = entityFetcher;
            if (staticEntity != null)
            {
                EntityFetcher = () => staticEntity;
            }

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
                TextColor = EntityFetcher().Color("rescolor"),
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

            effectsGrid = new EffectsGrid(EntityFetcher, consolas);

            squad = new Label
            {
                Text = $"{strings["squad"]}:",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 10, 10, 5),
                TextColor = Color.IndianRed,
                Font = consolas
            };

            squadgrid = new SquadGrid(EntityFetcher, consolas);

            #region enemy abilities

            var abilities = new VerticalStackPanel();
            var abilslabel = new Label()
            {
                Text = $"{strings["abilities"]}:",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 0, 0, 15),
                TextColor = Color.LightYellow,
                Font = consolas
            };
            abilities.Widgets.Add(abilslabel);

            foreach (var abil in EntityFetcher().Abilities)
            {
                var abillabel = new Label()
                {
                    Text = $"{strings[abil["name"].String]}:",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Myra.Graphics2D.Thickness(2),
                    TextColor = Color.LightYellow,
                    Font = consolas
                };

                abilities.Widgets.Add(abillabel);
            }

            #endregion

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

            if(side== Side.Left)
                panel.Widgets.Add(abilities);

            panel.Widgets.Add(squad);
            panel.Widgets.Add(squadgrid);

            return panel;
        }

        public override void Update(GameTime gameTime)
        {
            if (EntityFetcher == null)
                return;

            var strings = Game.Strings["Roguelike"];

            var entity = EntityFetcher();

            name.Text = entity.Name ?? strings[entity["name"].String];
            raceclass.Text = $"{strings[entity["race"].String]} - {strings[entity["class"].String]}";
            level.Text = $"{strings["level"]}: {entity["level"]}";
            exp.Text = $"{strings["exp"]}: {entity["exp"]}/{entity.Func("mexp").Number}";
            health.Text = $"{strings["health"]}: {entity["hp"]}/{entity["mhp"]}";
            resource.Text = $"{strings[entity["res"].String]}: {entity.Func("resstring").String}";
            damage.Text = $"{strings["damage"]}: {entity["mindmg"]}-{entity["maxdmg"]}";
            ad.Text = $"{strings["ad"]}: {entity["ad"]}";
            ap.Text = $"{strings["ap"]}: {entity["ap"]}";
            def.Text = $"{strings["def"]}: {entity["def"]}";
            mdef.Text = $"{strings["mdef"]}: {entity["mdef"]}";
            gold.Text = $"{strings["gold"]}: {entity["gold"]}";
            
            effectsGrid.Update(gameTime);
            squadgrid.Update(gameTime);
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }

        public override void Dispose()
        {
            EntityFetcher?.Invoke()?.Destroy();
            EntityFetcher = null;
        }
    }
}
