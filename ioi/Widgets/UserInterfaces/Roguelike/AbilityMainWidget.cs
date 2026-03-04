using ioi.Widgets.Base;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class AbilityMainWidget : ScreenWidget
    {
        private List<AbilityIcon> abilpanels = new();

        public AbilityMainWidget(GameHost game) : base(game)
        {
        }

        protected override Widget CreateWidget()
        {
            var grid = new Grid
            {
                //ColumnSpacing = 5,
                RowSpacing = 9,
                Width = (AbilityIcon.WidthMax + 10) * 4,
                Height = 200
            };


            for (int i = 0; i < 4; i++)
            {
                var abil = new AbilityIcon(Game, i);
                grid.Widgets.Add(abil);
                Grid.SetColumn(abil, i);
                Grid.SetRow(abil, 0);
                abilpanels.Add(abil);
            }

            for (int i = 0; i < 4; i++)
            {
                var abil = new AbilityIcon(Game, i, false);
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

        private class AbilityIcon : Panel
        {
            public static int WidthMax = 86;
            public static int HeightMax = 93;
            private bool _isSkill;

            public GameHost Game { get; }

            private Panel btn;

            private int _idx;
            private Image image;

            public AbilityIcon(GameHost game, int i, bool isSkill=true)
            {
                _isSkill = isSkill;
                Game = game;
                Width = WidthMax;
                Height = HeightMax;

                _idx = i+1;

                btn = new Panel()
                {
                    Height = 60 * 2,
                    Width = 54 * 2,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background=new SolidBrush(Color.Black),
                };
                btn.MouseEntered += Btn_MouseEntered;
                btn.MouseLeft += Btn_MouseLeft;
                btn.TouchDown += Btn_TouchDown;
                btn.TouchUp += Btn_TouchUp;

                btn.Widgets.Add(image = new Image()
                {
                    VerticalAlignment= VerticalAlignment.Center,
                    HorizontalAlignment= HorizontalAlignment.Center,
                });

                this.Widgets.Add(btn);
            }

            private void Btn_TouchUp(object sender, MyraEventArgs e)
                => Btn_MouseLeft(sender, e);

            private void Btn_TouchDown(object sender, MyraEventArgs e)
            {
                if (Game.GameState.Enemy == null)
                {
                    Game.GameState.Player.Entity.CastAbility(_idx);
                }
                else
                {
                    Game.World.CombatSystem.UseAbility(Game.GameState.Player.Entity, _idx, Game.GameState.Enemy);
                }

                Btn_MouseLeft(sender, e);
            }

            private void Btn_MouseLeft(object sender, MyraEventArgs e)
            {
                btn.Background = new SolidBrush(Color.Black);
            }

            private void Btn_MouseEntered(object sender, MyraEventArgs e)
            {
                btn.Background = new SolidBrush(new Color(25,25,25));
            }

            string prevTileset = "";
            int prevTileid = 0;

            public void Update(GameTime gameTime)
            {
                if (_isSkill)
                {
                    var abil = Game.GameState.Player.Entity.GetAbility(_idx);
                    if (abil != null)
                    {
                        var tileset = abil["tileset"].String;
                        var tileId = ((int)abil["tileid"].Number);

                        if (prevTileset != tileset || prevTileid != tileId)
                        {
                            var region = Game.GameState.Map.Tilesets[tileset].GetRegion(tileId);
                            image.Renderable = new TextureRegion(region.Texture, new Rectangle()
                            {
                                X = region.X,
                                Y = region.Y,
                                Width = region.Width,
                                Height = region.Height
                            });
                        }
                        image.Color = abil.Color("color");

                        prevTileset = tileset;
                        prevTileid = tileId;
                    }
                }
            }
        }
    }
}
