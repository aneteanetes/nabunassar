using FontStashSharp;
using ioi.Components;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.InfoList
{
    internal class InfoWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private ObjectMap[] _objects;
        private List<ObjectInfoRow> rows = new();
        private int currentRowIdx = 0;
        private VerticalStackPanel panel;

        public InfoWidget(GameHost game, ObjectMap[] objs) : base(game)
        {
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
            panel = new VerticalStackPanel
            {
                Width = 422,
                Height = 800,
                Background = new SolidBrush(Color.Black)
            };

            Refresh(_objects);

            rows.FirstOrDefault()?.Select();

            return panel;
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

        public override void Dispose()
        {
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

        internal void Refresh(IEnumerable<ObjectMap> objects)
        {
            rows.Clear();
            panel.Widgets.Clear();

            var strings = Game.Strings["Roguelike"];
            var largeBottomMargin = 20;

            var title = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 15, 10, largeBottomMargin),
                TextColor = Color.Cyan,
                Text = $"{strings["objects"]}:",
                Font = consolas
            };
            panel.Widgets.Add(title);

            foreach (var obj in objects)
            {
                if (obj == Game.GameState.Player)
                    continue;

                panel.Widgets.Add(AddRow(obj.Entity));
            }
        }
    }
}
