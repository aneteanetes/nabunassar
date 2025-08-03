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
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class AbilitiesWindow : TwoSideWindow
    {
        private Dictionary<Hero, Quad<AbilityView>> _worldAbilityViews = new();
        private Dictionary<Hero, Quad<AbilityView>> _battleAbilityViews = new();
        private FontSystem _fontBitterBold;
        private FontSystem _fontRetron;
        private ItemPanel _itemPanel;
        private bool _isCombatAbilities;

        public override bool IsModal => true;

        public AbilitiesWindow(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            _fontBitterBold = Content.LoadFont(Fonts.BitterBold);
            _fontRetron = Game.Content.LoadFont(Fonts.Retron);

            var itemViews = new List<ItemView>();
            foreach (var item in Game.GameState.Party.Inventory.Where(x => x.ItemSubtype == ItemSubtype.Tablet))
            {
                itemViews.Add(new ItemView(item, Content));
            }

            _itemPanel = new ItemPanel(itemViews, _fontBitterBold, null, null, TabletDoubleClick);
        }

        int sideWidth = 350;

        private void LoadAbilityViews()
        {
            foreach (var hero in Game.GameState.Party)
            {
                _worldAbilityViews[hero] = new Quad<AbilityView>();
                foreach (var worldAbility in hero.Creature.WorldAbilities)
                {
                    if (worldAbility == null)
                        continue;

                    var texture = Content.Load<Texture2D>(worldAbility.Icon);
                    var info = new AbilityView(worldAbility, texture);

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
        }

        protected override Widget LeftSide()
        {
            LoadAbilityViews();

            var tabs = new TabControl();
            tabs.TabButtonsHorizontalAligment = HorizontalAlignment.Center;
            tabs.Width = sideWidth;
            tabs.HorizontalAlignment = HorizontalAlignment.Center;

            var peacefullText = Game.Strings["UI"]["PeacefullAbilities"];
            var peacefullWidget = AbilitiesPanel(_worldAbilityViews);
            var peacefullTab = new TabItem(peacefullText, peacefullWidget);
            tabs.Items.Add(peacefullTab);

            var battleText = Game.Strings["UI"]["BattleAbilities"];
            var battleWidget = AbilitiesPanel(_battleAbilityViews);
            var battleTab = new TabItem(battleText, battleWidget);
            tabs.Items.Add(battleTab);

            tabs.SelectedIndexChanged += (s, e) =>
            {
                _isCombatAbilities = tabs.SelectedIndex != 0;
                FilterItems();
            };

            tabs.MinHeight = 550;

            return tabs;
        }

        private Widget AbilitiesPanel(Dictionary<Hero, Quad<AbilityView>> abilities)
        {
            var tabcontrol = new TabControl();
            tabcontrol.TabButtonsHorizontalAligment = HorizontalAlignment.Center;

            foreach (var hero in Game.GameState.Party)
            {
                var abilitiesPanel = HeroAbilitiesPanel(hero, abilities);
                var peacefullTab = new TabItem(hero.Name, abilitiesPanel);
                tabcontrol.Items.Add(peacefullTab);
            }

            _selectedHero = Game.GameState.Party.First;

            tabcontrol.SelectedIndexChanged += (s, e) =>
            {
                var idx = tabcontrol.SelectedIndex.Value;
                var party = Game.GameState.Party;
                _selectedArchetype = party[idx].Creature.Class;
                _selectedHero = party[idx];
                FilterItems();
            };

            return tabcontrol;

            //old realization

            //var panel = new VerticalStackPanel();

            //var scroll = new ScrollViewer
            //{
            //    ScrollMultiplier=40,
            //    Content = panel
            //};

            //foreach (var hero in Game.GameState.Party)
            //{
            //    panel.Widgets.Add(HeroAbilitiesPanel(hero, abilities));
            //}

            //return scroll;
        }

        private Widget HeroAbilitiesPanel(Hero hero, Dictionary<Hero,Quad<AbilityView>> abilities)
        {
            var panel = new VerticalStackPanel();

            var heroName = new Label()
            {
                Text = $"{hero.Name} ({Game.Strings["Enums/Archetypes"][hero.Creature.Class.ToString()+hero.Sex.ToString()]})",
                Font = _fontBitterBold.GetFont(22),
                Margin = new Myra.Graphics2D.Thickness(0,7)
            };

            panel.Widgets.Add(heroName);

            var skillPanels = new VerticalStackPanel()
            {
                Border = new SolidBrush(Color.White),
                BorderThickness = new Myra.Graphics2D.Thickness(1)
            };

            foreach (var abil in abilities[hero])
            {
                skillPanels.Widgets.Add(HeroAbilityPanel(abil));
            }

            panel.Widgets.Add(skillPanels);


            return panel;
        }

        private Widget HeroAbilityPanel(AbilityView view)
        {
            var iconpanel = new Iconpanel(view?.Texture, view?.Ability?.Name);
            iconpanel.Width = 425;

            var font = _fontBitterBold.GetFont(18);

            if (view != null)
            {
                var text = DrawText.Create(Game.Strings["GameTexts"]["Rank"])
                    .Append(": ")
                    .Append(view.Ability.Rank.GetName(Game));

                var ranklabel = new Label()
                {
                    Text = text.ToString(),
                    Font = font
                };
                iconpanel.Add(ranklabel);

                var dicePanel = new HorizontalStackPanel();

                var diceText = new Label()
                {
                    Text = $"{Game.Strings["GameTexts"]["Dice"]}: ",
                    Font  = font,
                    VerticalAlignment = VerticalAlignment.Center
                };
                dicePanel.Widgets.Add(diceText);

                var diceIcon = new DiceIcon(Game, view.Ability.Dice, true);
                diceIcon.VerticalAlignment = VerticalAlignment.Center;
                diceIcon.Size = 24;

                dicePanel.Height = diceIcon.Size;
                dicePanel.Widgets.Add(diceIcon);

                iconpanel.Add(dicePanel);

                iconpanel.MouseEntered += (s,e)=> Iconpanel_MouseEntered(view);
                iconpanel.MouseLeft += Iconpanel_MouseLeft;
            }

            return iconpanel;
        }

        private void Iconpanel_MouseLeft(object sender, EventArgs e)
        {
            ResetDescription();
        }

        private void Iconpanel_MouseEntered(AbilityView abilityView)
        {
            var model = abilityView.Ability;

            var roll = model.GetFormula();

            var text = DrawText.Create($"{Game.Strings["UI"]["Naming"]}: {model.Name}")
                .AppendLine()
                .AppendLine()
                .Append($"{Game.Strings["UI"]["Description"]}: {model.Description}")
                .AppendLine()
                .AppendLine()
                .Append($"{Game.Strings["UI"]["Formula"]}:")
                .AppendLine()
                .Append(roll.Result.ToFormula(false))
                .AppendLine()
                .Append($"{Game.Strings["UI"]["Complexity"]}:")
                .AppendLine()
                .AppendLine()
                .Append(roll.Complexity.ToFormula(false));

            SetDescription(text.ToString());
        }

        private void RefreshAbilitiesView()
        {
            var grid = this.Window.Content.As<Grid>();

            grid.Widgets.RemoveAt(0);

            var left = LeftSide();
            Grid.SetColumn(left, 0);

            grid.Widgets.Insert(0, left);
        }

        private Label _descriptionLabel;
        private string _descriptionDefaultText;
        private Archetype _selectedArchetype;
        private List<IconButton> _filterButtons;
        private DefaultButton _takeBtn;
        private Hero _selectedHero;

        protected override Widget RightSide()
        {
            var tabs = new TabControl();
            tabs.TabButtonsHorizontalAligment = HorizontalAlignment.Center;
            tabs.Width = sideWidth;
            //tabs.HorizontalAlignment = HorizontalAlignment.Center;

            var descriptionTabName = Game.Strings["UI"]["Descriptions"];
            _descriptionLabel = new Label()
            {
                Text = _descriptionDefaultText = Game.Strings["UI"]["FocusForHint"],
                Font = this._fontBitterBold.GetFont(20),
                Wrap = true
            };
            ResetDescription();
            var descriptionWidget = new ScrollViewer();
            descriptionWidget.Content = _descriptionLabel;
            var descriptionTab = new TabItem(descriptionTabName, descriptionWidget);
            tabs.Items.Add(descriptionTab);

            var tabletsName = Game.Strings["UI"]["Tablets"];
            var tabletsWidget = TabletsViewer();
            var tabletsTab = new TabItem(tabletsName, tabletsWidget);
            tabs.Items.Add(tabletsTab);

            tabs.MinHeight = 550;

            return tabs;
        }

        private void FilterItems()
        {
            _itemPanel.Filter(x => x.IsCombat == _isCombatAbilities && x.Archetype == _selectedArchetype);
        }

        private void ResetDescription()
        {
            _descriptionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _descriptionLabel.VerticalAlignment = VerticalAlignment.Center;
            _descriptionLabel.Text = _descriptionDefaultText;
        }

        private void SetDescription(string description)
        {
            _descriptionLabel.Text = description;
            _descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
            _descriptionLabel.VerticalAlignment = VerticalAlignment.Top;
        }

        private Widget TabletsViewer()
        {
            var panel = new Panel();
            //panel.Background = new SolidBrush(Color.Red);

            var tablets = _itemPanel;
            //tablets.Background=new SolidBrush(Color.Blue);
            panel.Widgets.Add(tablets);

            _takeBtn = new DefaultButton(Game.Strings["UI"]["Equip"]);
            _takeBtn.Click += TakeBtn_Click;
            _takeBtn.VerticalAlignment = VerticalAlignment.Bottom;
            panel.Widgets.Add(_takeBtn);

            return panel;
        }

        private void TakeBtn_Click(object sender, EventArgs e)
        {
            TabletDoubleClick(null, _itemPanel.SelectedItem);
        }

        private void TabletDoubleClick(Panel panel, Item item)
        {
            Equip();
        }

        private void Equip()
        {
            var item = _itemPanel.SelectedItem;
            _itemPanel.Remove(item);
            Game.GameState.Party.Inventory.RemoveItem(item);

            item.Destroy();

            var abilityName = item.AbilityName;
            var abilityModel = Game.DataBase.GetAbility(abilityName);

            if (abilityModel.IsCombat)
            {
            }
            else
            {
                var ability = abilityModel.CreateWorldAbility(Game, _selectedHero.Creature);

                var currentAbility = _selectedHero.Creature.WorldAbilities[abilityModel.Slot];
                if (currentAbility != null)
                    AddItem(currentAbility.ItemId);

                _selectedHero.Creature.WorldAbilities[abilityModel.Slot] = ability;
            }

            RefreshAbilitiesView();
        }

        private void AddItem(int itemId)
        {
            var item = Game.DataBase.GetItem(itemId);
            var view = new ItemView(item, Content);
            _itemPanel.AddItem(view);
            Game.GameState.Party.Inventory.AddItem(item);
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

        public override void Update(GameTime gameTime)
        {
            _takeBtn.Enabled = _itemPanel.SelectedItem != null;
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
