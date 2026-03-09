using FontStashSharp;
using Geranium.Reflection;
using ioi.Components;
using ioi.Entities.Struct;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.InfoList
{
    internal class ObjectList : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private DrawText _title;
        private int _width;
        private int _height;
        private GameEntity[] _objects;
        private List<ObjectInfoRow> rows = new();
        private int currentRowIdx = 0;
        private VerticalStackPanel panel;

        public ObjectList(GameHost game, GameEntity[] objs, int height = 800,int width=422, DrawText title = default) : base(game)
        {
            _title = title;
            _width = width;
            _height = height;
            _objects = objs;
            Position = new Vector2(14, 24);
        }

        public bool IsEmpty() => rows.Count == 0;

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
        }

        protected override Widget CreateWidget()
        {
            var scroll = new ScrollViewer()
            {
                Width = _width,
                Height = _height,
                Background = new SolidBrush(Color.Black)
            };
            panel = new VerticalStackPanel
            {
                Width = _width,
                //Height = _height,
                //Background = new SolidBrush(Color.Black)
            };

            Refresh(_objects);

            rows.FirstOrDefault()?.Select();

            scroll.Content = panel;
            return scroll;
        }

        private ObjectInfoRow AddRow(GameEntity gameEntity)
        {
            ObjectInfoRow row = null;
            if (gameEntity["type"].String == "item")
                row = new ItemRow(this,Game, gameEntity, consolas);

            if (row != default)
                rows.Add(row);

            return row;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }

        public void Up()
        {
            if (currentRowIdx == 0)
            {
                currentRowIdx = rows.Count - 1;
            }
            else
            {
                currentRowIdx--;
            }
            rows[currentRowIdx].Select();
        }

        public void Down()
        {
            if (currentRowIdx == rows.Count - 1)
            {
                currentRowIdx = 0;
            }
            else
            {
                currentRowIdx++;
            }

            rows[currentRowIdx].Select();
        }

        internal void UnselectAll(ObjectInfoRow objectInfoRow)
        {
            foreach (var row in rows)
            {
                if (row == objectInfoRow)
                {
                    currentRowIdx = rows.IndexOf(row);
                    continue;
                }

                row.Unselect();
            }
        }

        internal void Refresh(IEnumerable<GameEntity> objects)
        {
            rows.Clear();
            panel.Widgets.Clear();

            var strings = Game.Strings["Roguelike"];
            var largeBottomMargin = 20;

            bool drawTitle = true;
            if (_title != default && _title.ToString().IsEmpty())
                drawTitle = false;

            if (drawTitle)
            {
                var title = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Myra.Graphics2D.Thickness(0, 15, 10, largeBottomMargin),
                    TextColor = Color.Cyan,
                    Text = _title == default ? $"{strings["objects"]}:" : _title.ToString(),
                    Font = consolas
                };
                panel.Widgets.Add(title);
            }

            foreach (var obj in objects)
            {
                if (obj == Game.GameState.Player.Entity)
                    continue;

                panel.Widgets.Add(AddRow(obj));
            }
        }
    }
}
