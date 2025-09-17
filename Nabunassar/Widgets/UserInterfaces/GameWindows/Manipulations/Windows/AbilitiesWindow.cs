using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Struct;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class AbilitiesWindow : TwoSideWindow
    {
        private Dictionary<Hero, Quad<AbilityView>> _worldAbilityViews = new();
        private Dictionary<Hero, Quad<AbilityView>> _battleAbilityViews = new();
        private FontSystem _fontBitterBold;
        private FontSystem _fontBitterRegular;
        private FontSystem _fontRetron;
        private ItemsPanel _itemPanel;
        private bool _isCombatAbilities;
        private IBrush _defaultPanelBackground;
        private IImage _disableTexture;

        public override bool IsModal => true;

        public AbilitiesWindow(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            _fontBitterBold = Content.LoadFont(Fonts.BitterBold);
            _fontBitterRegular = Content.LoadFont(Fonts.BitterRegular);
            _fontRetron = Game.Content.LoadFont(Fonts.Retron);

            var itemViews = new List<ItemView>();
            foreach (var item in Game.GameState.Party.Inventory.Where(x => x.ItemSubtype == ItemSubtype.Tablet))
            {
                itemViews.Add(new ItemView(item, Content));
            }

            _itemPanel = new ItemsPanel(itemViews, _fontBitterBold, null, null, TabletDoubleClick,isShortView:true);
            _itemPanel.Filter(x => x.Archetype == Game.GameState.Party.First.Creature.Archetype);

            _defaultPanelBackground = new Panel().Background;

            _disableTexture = new TextureRegion(Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png"), new Rectangle(624, 336, 16, 16));
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

                    var pos = hero.Creature.WorldAbilities.GetQuadPosition(worldAbility);
                    _worldAbilityViews[hero][pos] = info;
                }

                _battleAbilityViews[hero] = new Quad<AbilityView>();
                foreach (var battleAbility in hero.Creature.BattleAbilities)
                {
                    if (battleAbility == null)
                        continue;

                    var texture = Content.Load<Texture2D>(battleAbility.Icon);
                    var info = new AbilityView(battleAbility, texture);

                    var pos = hero.Creature.BattleAbilities.GetQuadPosition(battleAbility);
                    _battleAbilityViews[hero][pos] = info;
                }
            }
        }

        private int _selectedAbilityTab = 0;
        private int _selectedHeroTab = 0;

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

            tabs.SelectedIndex = _selectedAbilityTab;
            tabs.SelectedIndexChanged += (s, e) =>
            {
                _isCombatAbilities = tabs.SelectedIndex != 0;
                _selectedAbilityTab = tabs.SelectedIndex.HasValue ? tabs.SelectedIndex.Value : 0;
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

            tabcontrol.SelectedIndex = _selectedHeroTab;

            tabcontrol.SelectedIndexChanged += (s, e) =>
            {
                var idx = tabcontrol.SelectedIndex.Value;
                var party = Game.GameState.Party;
                _selectedArchetype = party[idx].Creature.Archetype;
                _selectedHero = party[idx];
                _selectedHeroTab = tabcontrol.SelectedIndex.HasValue ? tabcontrol.SelectedIndex.Value : 0;
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
                Text = $"{hero.Name} ({Game.Strings["Enums/Archetypes"][hero.Creature.Archetype.ToString()+hero.Sex.ToString()]})",
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
            var iconpanel = new Iconpanel(view?.Texture);
            iconpanel.Width = 425;

            _iconpanels.Add(iconpanel);

            var font = _fontBitterBold.GetFont(18);

            if (view != null)
            {
                var grid = new Grid();
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8));
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));

                var descPanel = new VerticalStackPanel();
                Grid.SetColumn(descPanel, 0);
                grid.Widgets.Add(descPanel);

                var nameLabel = new Label()
                {
                    Font = font,
                    Text = view.Ability.Name,
                    Wrap = true
                };
                descPanel.Widgets.Add(nameLabel);

                var text = DrawText.Create(Game.Strings["GameTexts"]["Rank"])
                    .Append(": ")
                    .Append(view.Ability.AbilityRank.GetName(Game));

                var ranklabel = new Label()
                {
                    Text = text.ToString(),
                    Font = font
                };
                descPanel.Widgets.Add(ranklabel);

                var dicePanel = new HorizontalStackPanel();

                var diceText = new Label()
                {
                    Text = $"{Game.Strings["GameTexts"]["Dice"]}: ",
                    Font  = font,
                    VerticalAlignment = VerticalAlignment.Center
                };
                dicePanel.Widgets.Add(diceText);

                var diceIcon = new DiceIcon(Game, view.Ability.AbilityDice, true);
                diceIcon.VerticalAlignment = VerticalAlignment.Center;
                diceIcon.Size = 24;

                dicePanel.Height = diceIcon.Size;
                dicePanel.Widgets.Add(diceIcon);

                descPanel.Widgets.Add(dicePanel);

                var disableBtn = new Image()
                {
                    Renderable = _disableTexture,
                    Color = Globals.BaseColor,
                    Tooltip = Game.Strings["UI"]["Unequip"],
                    HorizontalAlignment = HorizontalAlignment.Right,
                    //VerticalAlignment = VerticalAlignment.Top
                };
                disableBtn.MouseEntered += (s, e) => disableBtn.Color = Globals.CommonColor;
                disableBtn.MouseLeft += (s, e) => disableBtn.Color = Globals.BaseColor;

                disableBtn.TouchDown += (s, e) => Unequip(iconpanel, view);

                Grid.SetColumn(disableBtn, 1);
                grid.Widgets.Add(disableBtn);

                iconpanel.Add(grid);

                //events

                iconpanel.MouseEntered += (s,e)=> Iconpanel_MouseEntered(view);
                iconpanel.MouseLeft += Iconpanel_MouseLeft;

                iconpanel.TouchDown += (s,e)=> Iconpanel_TouchDown(iconpanel,view);
            }

            return iconpanel;
        }

        private void Iconpanel_TouchDown(Iconpanel sender, AbilityView abilityView)
        {
            foreach (var iconPanel in _iconpanels)
            {
                iconPanel.Background = _defaultPanelBackground;
            }

            _selectedAbilityView = abilityView;
            _selectedIconpanel = sender;
            _selectedIconpanel.Background = _selectedIconpanel.OverBackground;

            ResetDescription();
            ShowDescription(abilityView);
        }

        private void ShowDescription(AbilityView abilityView)
        {
            var model = abilityView.Ability;

            var roll = model.GetFormula();

            var text = DrawText.Create($"{Game.Strings["UI"]["Naming"]}: {model.Name}")
                .AppendLine()
                .AppendLine()
                .Append(abilityView.Ability.GetSlotDescription(Game))
                .AppendLine()
                .AppendLine()
                .Append($"{Game.Strings["UI"]["Description"]}: {model.Description}")
                .AppendLine()
                .AppendLine()
                .Append($"{Game.Strings["UI"]["Formula"]}:")
                .AppendLine()
                .Append(roll.Result.ToFormula())
                .AppendLine();

            if (roll is RollResultComplexity complexityRoll)
            {
                text = text
                    .AppendLine()
                    .Append($"{Game.Strings["UI"]["Complexity"]}:")
                    .AppendLine()
                    .Append(complexityRoll.Complexity.ToFormula());
            }

            text = text.AppendLine();
            for (int i = 0; i < 5; i++)
            {
                text = text.AppendLine($"{Game.Strings["GameTexts"]["Rank"]} {i + 1}: {Game.Strings["AbilityDescriptions"][abilityView.Ability.SystemName + (i + 1)]}")
                    .AppendLine();
            }

            SetDescription(text.ToString());
        }

        private List<Iconpanel> _iconpanels = new();
        private Iconpanel _selectedIconpanel;
        private AbilityView _selectedAbilityView;

        private void Iconpanel_MouseLeft(object sender, MyraEventArgs e)
        {
            ResetDescription();

            if (_selectedIconpanel != null)
            {
                ShowDescription(_selectedAbilityView);
            }
        }

        private void Iconpanel_MouseEntered(AbilityView abilityView)
        {
            ShowDescription(abilityView);
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
        private DefaultButton _takeBtn;
        private Hero _selectedHero;

        protected override Widget RightSide()
        {
            var grid = new Grid();
            grid.MinHeight = 550;
            grid.Width = sideWidth+100;

            _descriptionLabel = new Label()
            {
                Text = _descriptionDefaultText = Game.Strings["UI"]["FocusForHint"],
                Font = this._fontBitterRegular.GetFont(18),
                Margin = new Thickness(3),
                Wrap = true
            };
            ResetDescription();

            var descriptionWidget = new ScrollViewer();
            descriptionWidget.Border=new SolidBrush(Color.White);
            descriptionWidget.BorderThickness = new Thickness(1);
            descriptionWidget.Content = _descriptionLabel;

            Grid.SetColumn(descriptionWidget, 0);
            Grid.SetRow(descriptionWidget, 0);
            grid.Widgets.Add(descriptionWidget);

            var tabletsWidget = _itemPanel;
            tabletsWidget.Border=new SolidBrush(Color.White);
            tabletsWidget.BorderThickness=new Thickness(1);

            tabletsWidget.ItemMouseEntered += TabletsWidget_ItemMouseEntered;
            tabletsWidget.ItemMouseLeft += TabletsWidget_ItemMouseLeft;
            tabletsWidget.ItemTouchDown += TabletsWidget_ItemTouchDown;

            Grid.SetColumn(tabletsWidget, 1);
            Grid.SetRow(tabletsWidget, 0);
            grid.Widgets.Add(tabletsWidget);

            _takeBtn = new DefaultButton(Game.Strings["UI"]["Equip"]);
            _takeBtn.Click += TakeBtn_Click;
            _takeBtn.VerticalAlignment = VerticalAlignment.Bottom;
            grid.Widgets.Add(_takeBtn);

            Grid.SetColumnSpan(_takeBtn,2);
            Grid.SetRow(_takeBtn, 1);

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));

            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 9.5f));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 0.5f));

            return grid;
        }

        private void TabletsWidget_ItemTouchDown(object sender, Item e)
        {
            TabletsWidget_ItemMouseEntered(sender, e);
            _selectedAbilityView = GetAbilityView(e.AbilityName);
        }

        private void TabletsWidget_ItemMouseLeft(object sender, Item e)
        {
            if (_selectedAbilityView == null)
            {
                ResetDescription();
            }
            else
            {
                ShowDescription(_selectedAbilityView);
            }
        }

        private void TabletsWidget_ItemMouseEntered(object sender, Item e)
        {
            ResetDescription();
            var abil = GetAbilityView(e.AbilityName);
            ShowDescription(abil);
        }

        private Dictionary<string, AbilityView> _abilityViewCache = new();

        private AbilityView GetAbilityView(string abilityName)
        {
            if(!_abilityViewCache.TryGetValue(abilityName, out var view))
            {
                var model = Game.DataBase.GetAbility(abilityName);
                var abil = model.Load(Game);
                var texture = Content.Load<Texture2D>(abil.Icon);
                view = new AbilityView(abil, texture);

                _abilityViewCache[abilityName] = view;
            }

            return view;
        }

        private void FilterItems()
        {
            _itemPanel.ResetSelectedItem();
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

        private void TakeBtn_Click(object sender, MyraEventArgs e)
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

            _itemPanel.ResetSelectedItem();

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
                    AddItemToInventory(currentAbility.ItemId);

                _selectedHero.Creature.WorldAbilities[abilityModel.Slot] = ability;
            }

            RefreshAbilitiesView();
        }

        private void Unequip(Iconpanel iconpanel, AbilityView view)
        {
            AddItemToInventory(view.Ability.ItemId);

            if (view.Ability.IsCombat)
            {
            }
            else
            {
                _selectedHero.Creature.WorldAbilities[view.Ability.Slot] = null;
            }

            RefreshAbilitiesView();
        }

        private void AddItemToInventory(int itemId)
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
            _itemPanel.Dispose();
            _abilityViewCache.Clear();
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
