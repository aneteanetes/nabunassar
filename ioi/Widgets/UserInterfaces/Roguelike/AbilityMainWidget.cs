using FontStashSharp;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class AbilityMainWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private List<AbilityPanel> abilpanels = new();

        public AbilityMainWidget(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(50);
        }

        protected override Widget CreateWidget()
        {
            var grid = new Grid
            {
                //ColumnSpacing = 5,
                RowSpacing = 9,
                Width = (AbilityPanel.WidthMax + 10) * 4,
                Height = 200
            };


            for (int i = 0; i < 4; i++)
            {
                var abil = new AbilityPanel(Game,consolas,i);
                grid.Widgets.Add(abil);
                Grid.SetColumn(abil, i);
                Grid.SetRow(abil, 0);
                abilpanels.Add(abil);
            }

            for (int i = 0; i < 4; i++)
            {
                var abil = new AbilityPanel(Game, consolas,i,false);
                grid.Widgets.Add(abil);
                Grid.SetColumn(abil, i);
                Grid.SetRow(abil, 1);
                abilpanels.Add(abil);
            }

            return grid;
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = 1485;
            widget.Top = 872;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var abilpanel in abilpanels)
            {
                abilpanel.Update(gameTime); 
            }
        }

        private class AbilityPanel : Panel
        {
            public static int WidthMax = 86;
            public static int HeightMax = 93;
            private bool _isSkill;

            public GameHost Game { get; }

            private Label _label;

            private int _idx;

            public AbilityPanel(GameHost game, SpriteFontBase consolas, int i, bool isSkill=true)
            {
                _isSkill = isSkill;
                Game = game;
                Width = WidthMax;
                Height = HeightMax;

                _idx = i+1;

                _label = new Label()
                {
                    Font = consolas,
                    Text = "",
                    TextColor = Color.CadetBlue,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                this.Widgets.Add(_label);
            }

            public void Update(GameTime gameTime)
            {
                if (_isSkill)
                {
                    var abil = Game.GameState.Player.Entity.GetAbility(_idx);
                    if (abil != null)
                    {
                        _label.Text = abil["icon"].String;
                        _label.TextColor = abil.Color("color");
                    }
                }
            }
        }
    }
}
