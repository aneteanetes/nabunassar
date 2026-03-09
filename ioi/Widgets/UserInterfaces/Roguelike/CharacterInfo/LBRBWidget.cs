using Geranium.Reflection;
using ioi.Entities.Data.Items;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.CharacterInfo
{
    internal class LBRBWidget : Grid
    {
        public GameHost Game { get; }
        private LBRBItem[] _items;
        private List<Image> _images = new();
        private int _idx;
        private SolidBrush gold;
        private SolidBrush red;
        private SolidBrush black;
        private SolidBrush gray;

        public event EventHandler<LBRBItem> OnChanged;

        public LBRBWidget(GameHost game, int width, params LBRBItem[] items)
        {
            Game = game;
            _items = items;
            _idx = 0;

            var size = game.CellSize.Scale(1.5);

            var region = Game.GameState.Tilesets["Consolas"].GetRegion(603).ToMyraRegion();
            region.Color = Color.Goldenrod;
            this.Border = region;
            this.BorderThickness = new Myra.Graphics2D.Thickness(0, 0, 0, 34);

            this.Width = width;
            this.Height = size.Y+size.Y;

            gold = new SolidBrush(Color.Goldenrod);
            red =new SolidBrush(Color.IndianRed);
            black = new SolidBrush(Color.Black);
            gray = new SolidBrush(new Color(25, 25, 25));

            HorizontalAlignment = HorizontalAlignment.Center;

            var i = 0;
            foreach (var item in items)
            {
                var img = new Image()
                {
                    Width = size.X,
                    Height = size.Y,
                    Renderable = Game.GameState.Tilesets[item.Tileset].GetRegion(item.TileId).ToMyraRegion(),
                    VerticalAlignment= VerticalAlignment.Center,
                    HorizontalAlignment= HorizontalAlignment.Center,
                    //Margin=new Myra.Graphics2D.Thickness(10,0,10,0),
                    Color = Color.Goldenrod,
                    Border=black,
                    BorderThickness=new Myra.Graphics2D.Thickness(1)
                };
                img.MouseEntered += Img_MouseEntered;
                img.MouseLeft += Img_MouseLeft;
                img.TouchUp += (s,e)=>
                {
                    _idx = Array.IndexOf(_items, item);
                    LBRBWidget_OnChanged(img, item);
                };

                Grid.SetColumn(img, i);
                i++;

                _images.Add(img);

                this.Widgets.Add(img);
            }

            OnChanged += LBRBWidget_OnChanged;
            LBRBWidget_OnChanged(this, _items.FirstOrDefault());
        }

        private void Img_MouseLeft(object sender, MyraEventArgs e)
        {
            var imgIdx = _images.IndexOf(sender.As<Image>());
            if (imgIdx != _idx)
            {
                sender.As<Image>().Background = black;
                sender.As<Image>().Border = black;
            }
        }

        private void Img_MouseEntered(object sender, MyraEventArgs e)
        {
            sender.As<Image>().Background = gray;
            sender.As<Image>().Border = gold;
        }

        private void LBRBWidget_OnChanged(object sender, LBRBItem e)
        {
            _images.ForEach(img =>
            {
                img.Background = black;
                img.Border = black;
            });
            _images[_idx].Background = gray;
            _images[_idx].Border = gold;
        }

        public void Update(GameTime gameTime)
        {
            var controls = Game.World.PlayerControlSystem.GetControls();

            if (controls.MenuLB.WasPressed())
            {
                if (_idx == 0)
                {
                    _idx = _items.Length - 1;
                }
                else
                {
                    _idx--;
                }

                OnChanged?.Invoke(this,_items[_idx]);
            }


            if (controls.MenuRB.WasPressed())
            {
                if (_idx == _items.Length - 1)
                {
                    _idx = 0;
                }
                else
                {
                    _idx++;
                }

                OnChanged?.Invoke(this, _items[_idx]);
            }
        } 
    }
}
