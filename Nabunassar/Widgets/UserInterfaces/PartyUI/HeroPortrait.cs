using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Widgets.UserInterfaces.Combat;
using Nabunassar.Widgets.Views;
using Nabunassar.Widgets.Views.StatusEffects;

namespace Nabunassar.Widgets.UserInterfaces.PartyUI
{
    internal class HeroPortrait : Grid, IFeatured
    {

        private int currentFrame = 0;
        private List<TextureRegion> _frames = new();
        private NabunassarGame _game;
        private Hero _hero;
        private Image _portrait;
        private bool _isInited;
        private Grid _effectGrid;
        private List<StatusEffectWidget> _effectWidgets = new();
        private HPLine _hpLine;
        private ArmorClassWidget _acWidget;
        private WillPointsWidget _wpWidget;

        public HeroPortrait(NabunassarGame game, Hero hero)
        {
            _game = game;
            _hero = hero;

            var imageWidth = 75;
            var imageHeight = ((int)Math.Round(0.88 * imageWidth));
            var effectGridWidth = 50;

            this.Width = (imageWidth + 10) + effectGridWidth;
            this.Height = 130;

            ColumnsProportions.Add(new Proportion(ProportionType.Part, 0.69f));
            ColumnsProportions.Add(new Proportion(ProportionType.Part, 0.31f));

            var roundedTexture = game.Content.LoadTexture("Assets/Images/Borders/conditionbackground.png");
            var roundedBackground = roundedTexture.NinePatch(16);

            var portraitPanel = new VerticalStackPanel();
            portraitPanel.VerticalAlignment = VerticalAlignment.Stretch;
            portraitPanel.HorizontalAlignment = HorizontalAlignment.Stretch;

            portraitPanel.Background = roundedBackground;

            var texture = game.Content.Load<Texture2D>("Assets/Tilesets/" + hero.Tileset);
            var imageBorder = new Panel()
            {
                Width = imageWidth + 10,
                Height = imageHeight + 10,
                Background = roundedBackground,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 3, 0, 0)
            };
            _portrait = new Image()
            {
                Renderable = new TextureRegion(texture, new Rectangle(0, 31, 16, 13)),
                Width = imageWidth,
                Height = imageHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Background = new SolidBrush(Color.Red)
            };

            _frames.Add(new TextureRegion(texture, new Rectangle(0, 31, 16, 13)));
            _frames.Add(new TextureRegion(texture, new Rectangle(16, 31, 16, 13)));
            _frames.Add(new TextureRegion(texture, new Rectangle(32, 31, 16, 13)));
            _frames.Add(new TextureRegion(texture, new Rectangle(48, 31, 16, 13)));

            imageBorder.Widgets.Add(_portrait);
            portraitPanel.Widgets.Add(imageBorder);

            portraitPanel.Widgets.Add(_hpLine = new HPLine(game, hero.Creature, 60));
            
            Grid statGrid = FillStatsPoints(game, hero);
            portraitPanel.Widgets.Add(statGrid);

            Grid.SetColumn(portraitPanel, 0);
            Widgets.Add(portraitPanel);

            _effectGrid = new Grid();
            //_effectGrid.Background = new SolidBrush(Color.Green);
            _effectGrid.HorizontalAlignment = HorizontalAlignment.Right;
            _effectGrid.VerticalAlignment = VerticalAlignment.Top;

            _effectGrid.Width = effectGridWidth;
            _effectGrid.Height = Height;
            //_effectGrid.Left = _effectGrid.Width.Value;

            Grid.SetColumn(_effectGrid, 1);
            Widgets.Add(_effectGrid);
        }

        private Grid FillStatsPoints(NabunassarGame game, Hero hero)
        {
            var statGrid = new Grid();
            statGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4f));
            statGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1f));
            statGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4f));
            statGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1f));

            _acWidget = new ArmorClassWidget(game, hero.Creature);
            _acWidget.HorizontalAlignment = HorizontalAlignment.Center;
            _acWidget.Margin=new Myra.Graphics2D.Thickness(3,0,0,0);
            Grid.SetColumn(_acWidget, 0);
            statGrid.Widgets.Add(_acWidget);

            _wpWidget = new WillPointsWidget(game, hero.Creature);
            _wpWidget.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(_wpWidget, 2);
            statGrid.Widgets.Add(_wpWidget);
            return statGrid;
        }

        public void Update(GameTime gameTime)
        {
            if (_hero.Creature.Effects.IsChanged || !_isInited)
            {
                _isInited = true;

                _effectWidgets.Clear();
                _effectGrid.Widgets.Clear();

                var effects = _hero.Creature.Effects.OrderBy(x=>x.Type).ToArray();

                var column = 0;
                var row = 0;

                foreach (var effect in effects)
                {
                    var effectWidget = new StatusEffectWidget(_game, effect, new MonoGame.Extended.Size(25, 25));
                    _effectGrid.Widgets.Add(effectWidget);
                    _effectWidgets.Add(effectWidget);

                    Grid.SetRow(effectWidget, row);
                    Grid.SetColumn(effectWidget, column);

                    effectWidget.MouseEntered += EffectWidget_MouseEntered;

                    row++;

                    if (row >= 5)
                    {
                        row = 0;
                        column++;
                    }
                }
            }

            _hpLine.Update(gameTime);

            foreach (var effectWidget in _effectWidgets)
            {
                effectWidget.Update(gameTime);
            }

            UpdateStats(gameTime);

            return;
            AnimatePortraits(gameTime);
        }

        private void UpdateStats(GameTime gameTime)
        {
            _acWidget.Update(gameTime);
            _wpWidget.Update(gameTime);
        }

        private void EffectWidget_MouseEntered(object sender, MyraEventArgs e)
        {
            Console.WriteLine();
        }

        private void AnimatePortraits(GameTime gameTime)
        {
            if (this.CanUpdate(gameTime, TimeSpan.FromSeconds(0.3)))
            {
                currentFrame++;
                if (currentFrame == _frames.Count)
                    currentFrame = 0;

                _portrait.Renderable = _frames[currentFrame];
            }
        }
    }
}
