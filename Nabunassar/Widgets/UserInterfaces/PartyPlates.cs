using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.PartyUI;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class PartyPlates : ScreenWidget
    {
        public override bool IsRemovable => false;

        protected override bool IsMouseMovementAvailableWithThisActivedWidget => true;

        public PartyPlates(NabunassarGame game) : base(game)
        {
        }

        private List<HeroPortrait> _heroPortraits = new();

        protected override Widget CreateWidget()
        {
            var grid = new Grid();
            grid.RowSpacing = 8;
            grid.Margin = new Myra.Graphics2D.Thickness(10,10,0,0);
            grid.VerticalAlignment = VerticalAlignment.Top;

            var i = 0;

            foreach (var hero in Game.GameState.Party)
            {
                var portrait = new HeroPortrait(Game, hero);
                _heroPortraits.Add(portrait);
                grid.Widgets.Add(portrait);
                Grid.SetRow(portrait, i);
                i++;
            }

            return grid;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsGameActive)
                return;

            foreach (var heroPortrait in _heroPortraits)
            {
                heroPortrait.Update(gameTime);
            }
        }
    }
}
