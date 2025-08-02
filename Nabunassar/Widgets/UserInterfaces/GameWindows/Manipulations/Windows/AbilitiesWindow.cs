using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Affects;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class AbilitiesWindow : TwoSideWindow
    {
        private Dictionary<Hero, Quad<AbilityView>> _worldAbilityViews = new();
        private Dictionary<Hero, Quad<AbilityView>> _battleAbilityViews = new();
        private FontSystem _fontBitterBold;

        public override bool IsModal => true;

        public AbilitiesWindow(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            foreach (var hero in Game.GameState.Party)
            {
                _worldAbilityViews[hero] = new Quad<AbilityView>();
                foreach (var worldAbility in hero.Creature.WorldAbilities)
                {
                    if (worldAbility == null)
                        continue;

                    var texture = Content.Load<Texture2D>(worldAbility.Icon);
                    var info = new AbilityView(worldAbility,texture);

                    var pos = hero.Creature.WorldAbilities.GetPosition(worldAbility);
                    _worldAbilityViews[hero][pos] = info;
                }

                _battleAbilityViews[hero] = new Quad<AbilityView>();
                foreach (var battleAbility in hero.Creature.BattleAbilities)
                {
                    if (battleAbility == null)
                        continue;

                    var texture = Content.Load<Texture2D>(battleAbility.Icon);
                    var info = new AbilityView(battleAbility, texture);

                    var pos = hero.Creature.BattleAbilities.GetPosition(battleAbility);
                    _battleAbilityViews[hero][pos] = info;
                }
            }

            _fontBitterBold = Content.LoadFont(Fonts.BitterBold);
        }

        protected override Widget LeftSide()
        {
            var tabs = new TabControl();
            tabs.Width = 450;

            var peacefullText = Game.Strings["UI"]["PeacefullAbilities"];
            var peacefullWidget = WorldAbilitiesPanel();
            var peacefullTab = new TabItem(peacefullText, peacefullWidget);
            tabs.Items.Add(peacefullTab);

            var battleText = Game.Strings["UI"]["BattleAbilities"];
            var battleWidget = BattleAbilitiesPanel();
            var battleTab = new TabItem(battleText, battleWidget);
            tabs.Items.Add(battleTab);

            return tabs;
        }

        private Widget WorldAbilitiesPanel()
        {
            var panel = new VerticalStackPanel();

            var scroll = new ScrollViewer
            {
                Content = panel
            };

            foreach (var hero in Game.GameState.Party)
            {
                panel.Widgets.Add(HeroAbilitiesPanel(hero));
            }

            return scroll;
        }

        private Widget BattleAbilitiesPanel()
        {
            return new Panel();
        }

        private Widget HeroAbilitiesPanel(Hero hero)
        {
            var panel = new VerticalStackPanel();

            var heroName = new Label()
            {
                Text = hero.Name,
                Font = _fontBitterBold.GetFont(22),
                BorderThickness = new Myra.Graphics2D.Thickness(0, 0, 0, 1),
                Border = new SolidBrush(Color.White),
            };

            panel.Widgets.Add(heroName);

            foreach (var abil in _worldAbilityViews[hero])
            {
                panel.Widgets.Add(HeroAbilityPanel(abil));
            }

            return panel;
        }

        private Widget HeroAbilityPanel(AbilityView view)
        {
            var iconpanel = new Iconpanel(view?.Texture, view?.Ability?.Name);

            if (view != null)
            {
                var text = DrawText.Create(Game.Strings["GameTexts"]["Rank"])
                    .Append(": ")
                    .Append(view.Ability.Rank.GetName(Game));

                var ranklabel = new Label()
                {
                    Text = text.ToString()
                };
                iconpanel.Widgets.Add(ranklabel);

                var dicePanel = new HorizontalStackPanel();

                var diceText = new Label()
                {
                    Text = $"{Game.Strings["GameTexts"]["Dice"]}: "
                };
                dicePanel.Widgets.Add(diceText);

                var diceIcon = new DiceIcon(Game, view.Ability.Dice, true);
                diceIcon.Size = 32;
                dicePanel.Widgets.Add(diceIcon);

                iconpanel.Widgets.Add(dicePanel);
            }

            return iconpanel;
        }

        protected override Widget RightSide()
        {
            return new Panel()
            {
                Width = 450
            };
        }

        protected override void InitWindow(Window window)
        {
            StandartWindowTitle(window, Game.Strings["UI"]["PartyAbilities"]);
            window.MaxHeight = 550;
        }

        public override void Dispose()
        {
            ControlPanel.CloseAbility();
            base.Dispose();
        }

        private class AbilityView
        {
            public AbilityModel Ability { get; private set; }

            public TextureRegion Texture { get; private set; }

            public AbilityView(AbilityModel ability, Texture2D texture)
            {
                Ability = ability;
                Texture = new TextureRegion(texture);
            }
        }
    }
}
