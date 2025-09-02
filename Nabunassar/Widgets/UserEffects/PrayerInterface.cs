using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Effects.Boons;
using Nabunassar.Entities.Data.Effects.PartyEffects;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Shaders.Blooming;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.Views.DescriptionTolltip;
using Nabunassar.Widgets.Views.StatusEffects;
using SharpFont;

namespace Nabunassar.Widgets.UserEffects
{
    internal class PrayerInterface : ScreenWidget
    {
        private PrayerAbility _prayerAbility;
        private List<GodImage> godsImages = new();
        private Dictionary<int, Point> positions = new()
        {
            {1,new Point(1150,250) },
            {2,new Point(1150,425) },
            {3,new Point(1150,600) },
            {4,new Point(1150,775) },
            {5,new Point(950,875) },
            {6,new Point(740,875) },
            {7,new Point(530,775) },
            {8,new Point(530,600) },
            {9,new Point(530,425) },
            {10,new Point(530,250) },
            {11,new Point(740,180) },
            {12,new Point(950,180) },
        };
        private BloomShader _bloom;
        private FontSystem _retron;
        private Label _godName;
        private VerticalStackPanel _description;
        private Gods? _selected;
        private Image _selectedImage;
        Panel _mainPanel = null;
        DescriptionPanel _judgePanel;
        DescriptionPanel _blessingPanel;
        DescriptionPanel _blessingEffectPanel1;
        DescriptionPanel _blessingEffectPanel2;
        private GameButton _acceptBtn;

        public override bool IsModal => true;

        public PrayerInterface(NabunassarGame game, PrayerAbility ability) : base(game)
        {
            _prayerAbility = ability;
            game.RemoveDesktopWidgets<TitleWidget>();
        }

        public override void LoadContent()
        {
            _bloom = new BloomShader(Game, true, BloomShader.BloomPresets.Wide);
            _bloom.Enable();

            _retron = Content.LoadFont(Fonts.Retron);

            var gods = GetAvailableGods();

            for (int i = 0; i < gods.Length; i++)
            {
                var god = gods[i];
                Point pos = GetPositionByGod(((int)god));
                var texture = Content.LoadTexture($"Assets/Images/Icons/Gods/{god}.png");
                godsImages.Add(new GodImage(texture, pos));
            }
        }

        private Gods[] GetAvailableGods()
        {
            if (_prayerAbility.AbilityRank.Value >= 5)
                return typeof(Gods).GetAllValues<Gods>().ToArray();

            return Game.GameState.Location.GetAvailableGods().ToArray();
        }

        private Point GetPositionByGod(int god)
        {
            var pos = positions[god];
            pos.X += 50;
            pos.Y -= 50;

            return pos;
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardExtended.GetState().WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }

            if (_blessingEffectPanel1 != null)
                _blessingEffectPanel1.Top = _blessingPanel.Top + _blessingPanel.Bounds.Height + 30;
            if (_blessingEffectPanel2 != null)
                _blessingEffectPanel2.Top = _blessingEffectPanel1.Top + _blessingEffectPanel1.Bounds.Height + 30;

            var mouse = MouseExtended.GetState();

            godsImages.ForEach(x => x.Color = Color.White);

            var intersected = godsImages.FirstOrDefault(x => x.Bounds.Intersects(new Rectangle(mouse.Position, new Point(1, 1))));
            if (intersected != null)
            {
                intersected.Color = Color.BlueViolet;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //var sb = Game.BeginDraw(false);

            //foreach (var god in godsImages)
            //{
            //    sb.Draw(background, new Rectangle(god.Bounds.X, god.Bounds.Y, 128, 128), Color.White);
            //    sb.Draw(god.Texture, new Rectangle(god.Bounds.X, god.Bounds.Y, 128, 128), god.Color);
            //}

            //sb.End();
        }

        protected override Widget CreateWidget()
        {
            var panel = _mainPanel = new Panel() { Width = Game.Resolution.Width, Height = Game.Resolution.Height };
            panel.Background = new SolidBrush(new Color(Color.Black, 190));

            panel.TouchDown += Panel_TouchDown;

            var descriptionPanel = new Panel()
            {
                Background = ScreenWidgetWindow.WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 400,
                Height = 500,
                DragDirection = DragDirection.None,
            };


            var gods = typeof(Gods).GetAllValues<Gods>()
                .ToArray();

            var imgPanel = new HorizontalStackPanel();
            imgPanel.HorizontalAlignment = HorizontalAlignment.Center;
            imgPanel.VerticalAlignment = VerticalAlignment.Top;
            imgPanel.Top = 250;

            for (int i = 0; i < 12; i++)
            {
                var god = gods[i];
                Point pos = GetPositionByGod(((int)god));
                var texture = Content.LoadTexture($"Assets/Images/Icons/Gods/Runes/{god}.png");

                var img = new Image()
                {
                    Renderable = new TextureRegion(texture),
                    Margin = new Myra.Graphics2D.Thickness(25),
                    Height = 60 * 2,
                    Width = 54 * 2,
                };
                img.Height += 25;
                img.Width += 25;
                img.MouseEntered += (s, e) =>
                {
                    Select(img, god);
                };
                img.MouseLeft += (s, e) =>
                {
                    Select(img, null);
                };
                img.TouchDown += (s, e) =>
                {
                    e.StopPropagation();
                    Select(img, god, true);
                };
                imgPanel.Widgets.Add(img);
            }

            var selectText = new Label()
            {
                Text = Game.Strings["GameTexts"]["Select God"] + ":",
                Font = _retron.GetFont(48),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Top = 100,
                TextColor = Globals.BaseColor
            };
            panel.Widgets.Add(selectText);

            _godName = selectText.Clone().As<Label>();
            _godName.Text = "";
            _godName.Top = 165;
            panel.Widgets.Add(_godName);

            _description = new VerticalStackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Top = 50
            };
            AddRankDescriptions();
            UpdJudjement();
            UpdBlessing();
            Worship();

            panel.Widgets.Add(_description);

            panel.Widgets.Add(_acceptBtn = new GameButton(Game, Game.Strings["GameTexts"]["DoPray"])
            {
                VerticalAlignment= VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Top = -150,
                OnClick = (button, args) =>
                {
                    args.StopPropagation();
                    _prayerAbility.CastPrayer(_selected.Value);
                    this.Close();
                }
            });
            _acceptBtn.Enabled = false;

            panel.Widgets.Add(imgPanel);

            return panel;
        }

        private void UpdJudjement(Gods? godValue=null)
        {
            var judg = new Judgment(Game, godValue, _prayerAbility.AbilityRank, _prayerAbility.AbilityDice);
            var desc = judg.GetDescription();

            if (_judgePanel != null)
            {
                _mainPanel.Widgets.Remove(_judgePanel);
            }
            _mainPanel.Widgets.Add(_judgePanel = new DescriptionPanel(Game, desc)
            {
                Left = 200,
                Top = 450
            });
        }

        private void UpdBlessing(Gods? godValue = null)
        {
            var data = Description.Create(Game.Strings["GameTexts"]["Blessing"].ToString(), Color.Orange);
            data.Append(DescriptionPosition.Right, $"{Game.Strings["GameTexts"]["Rank"]} {_prayerAbility.AbilityRank.Value}", Color.Gray, data.TextSizeTitle);
            data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Boon"], Color.Gray);

            var god = godValue ?? _selected;

            var descriptionToken = god == null
                ? Game.Strings["GameTexts"]["BlessingOfGods"]
                : Game.Strings["Effects/EffectDescriptions"]["BlessingOf" + god.Value.ToString()];

            var text = descriptionToken.ToString()
                .Replace("{RankDice}", _prayerAbility.AbilityRank.AsDice().ToString());

            data.Append(DescriptionPosition.Center, text);

            if (_blessingPanel != null)
            {
                _mainPanel.Widgets.Remove(_blessingPanel);
            }
            _mainPanel.Widgets.Add(_blessingPanel = new DescriptionPanel(Game, data)
            {
                Top = 450,
                Left = 1400
            });

            if (god != null)
            {
                var additional1 = BlessingAdditionalEffect(god.Value);
                if (_blessingEffectPanel1 != null)
                {
                    _mainPanel.Widgets.Remove(_blessingEffectPanel1);
                }
                if (additional1 != null)
                {
                    additional1.Top = _blessingPanel.Top + _blessingPanel.Bounds.Height + 30;
                    additional1.Left = 1400;

                    _mainPanel.Widgets.Add(_blessingEffectPanel1 = additional1);
                }

                var additional2 = BlessingAdditionalEffect2(god.Value);
                if (_blessingEffectPanel2 != null)
                {
                    _mainPanel.Widgets.Remove(_blessingEffectPanel2);
                }
                if (additional2 != null)
                {
                    additional2.Top = _blessingEffectPanel1.Top + _blessingEffectPanel1.Bounds.Height + 30;
                    additional2.Left = 1400;

                    _mainPanel.Widgets.Add(_blessingEffectPanel2 = additional2);
                }
                else _blessingEffectPanel2 = null;
            }
        }

        private DescriptionPanel BlessingAdditionalEffect(Gods god)
        {
            DescriptionBuilder data = null;

            switch (god)
            {
                case Gods.Nasho:
                    data = Description.Create(Game.Strings["GameTexts"]["Soar"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Boon"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Soar"]);
                    break;
                case Gods.Sabu:
                    data = Description.Create(Game.Strings["GameTexts"]["Defence"], "#6285de".AsColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Characteristic"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Defence"]);
                    break;
                case Gods.Rohati:
                    data = Description.Create(Game.Strings["GameTexts"]["Disease"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Condition"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Disease"]);
                    break;
                case Gods.Nisa:
                    data = Description.Create(Game.Strings["GameTexts"]["Bleeding"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Condition"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Bleeding"]);
                    break;
                case Gods.Haya:
                    data = Description.Create(Game.Strings["GameTexts"]["MagicOfLife"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["MagicSchool"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["MagicOfLife"]);
                    break;
                case Gods.Ailul:
                    data = Description.Create(Game.Strings["GameTexts"]["Rage"], Color.Red);
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Condition"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Rage"]);
                    break;
                case Gods.Tamus:
                    data = Description.Create(Game.Strings["GameTexts"]["Machine"], Color.Orange);
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Characteristic"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Machine"]);
                    break;
                case Gods.Shamadj:
                    data = Description.Create(Game.Strings["GameTexts"]["SeaWater"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Boon"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["SeaWater"].ToString().Replace("{RankDice}", _prayerAbility.AbilityRank.AsDice().ToString()));
                    break;
                case Gods.Aval:
                    data = Description.Create(Game.Strings["GameTexts"]["MagicOfLight"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["MagicSchool"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["MagicOfLight"]);
                    break;
                case Gods.Ziran:
                    return null;
                case Gods.Teshrin:
                    data = Description.Create(Game.Strings["GameTexts"]["Night"], god.GodColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["TimesOfDay"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Night"]);
                    break;
                case Gods.Nergal:
                    data = Description.Create(Game.Strings["GameTexts"]["MortalHit"], "#821ed9".AsColor());
                    data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Characteristic"], Color.Gray);
                    data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["MortalHit"]);
                    data.SetProportion(0.9, 0.1);
                    break;
                default:
                    break;
            }          

            return new DescriptionPanel(Game, data);
        }

        private DescriptionPanel BlessingAdditionalEffect2(Gods god)
        {
            if (god != Gods.Rohati)
                return null;

            var data = Description.Create(Game.Strings["GameTexts"]["Poison"], "#40FD14".AsColor());
            data.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Condition"], Color.Gray);
            data.Append(DescriptionPosition.Center, Game.Strings["Effects/EffectDescriptions"]["Poison"]);

            return new DescriptionPanel(Game, data);
        }

        private void Worship()
        {
            var worship = new Worship(Game);

            _mainPanel.Widgets.Add(new DescriptionPanel(Game, worship.GetDescription())
            {
                Left = 200,
                Top = 625
            });
        }

        private void AddRankDescriptions()
        {
            var formulaText = _prayerAbility.GetFormula().Complexity.ToFormula().ToString();

            var text = DrawText.Create($"{Game.Strings["GameTexts"]["Rank"]} 1: ")
                .Append(Game.Strings["GameTexts"]["PrayerRank1"].ToString().Replace("{formula}", formulaText))
                .Append(JudgementText(0));

            var labelRank1 = new Label()
            {
                Text = text.ToString(),
                TextColor = Globals.BaseColor,
                Wrap = true,
                Width = ((int)(Game.Resolution.Width*0.4)),
                Font = _retron.GetFont(32),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            _description.Widgets.Add(labelRank1);

            var labelRank2 = labelRank1.CloneAs<Label>();
            labelRank2.Text = DrawText.Create($"{Game.Strings["GameTexts"]["Rank"]} 2: {Game.Strings["GameTexts"]["PrayerRank2"]}")
                .Append(JudgementText(2)).ToString();
            labelRank2.TextColor = GetColorForRank(2);
            _description.Widgets.Add(labelRank2);

            var labelRank3 = labelRank1.CloneAs<Label>();
            labelRank3.Text = $"{Game.Strings["GameTexts"]["Rank"]} 3: {Game.Strings["GameTexts"]["PrayerRank3"]}";
            labelRank3.TextColor = GetColorForRank(3);
            _description.Widgets.Add(labelRank3);

            var labelRank4 = labelRank1.CloneAs<Label>();
            labelRank4.Text = DrawText.Create($"{Game.Strings["GameTexts"]["Rank"]} 4: {Game.Strings["GameTexts"]["PrayerRank4"]}")
                .Append(BlessingText()).ToString();
            labelRank4.TextColor = GetColorForRank(4);
            _description.Widgets.Add(labelRank4);

            var labelRank5 = labelRank1.CloneAs<Label>();
            labelRank5.Text = $"{Game.Strings["GameTexts"]["Rank"]} 5: {Game.Strings["GameTexts"]["PrayerRank5"]}";
            labelRank5.TextColor = GetColorForRank(5);
            _description.Widgets.Add(labelRank5);
        }

        private Color GetColorForRank(int  rank)
        {
            return _prayerAbility.AbilityRank.Value >= rank ? Globals.BaseColor : Color.Gray;
        }

        private DrawText JudgementText(int rank)
        {
            var color = IsAvailableByRank(rank)
                ? Color.Yellow
                : "#a6a86f".AsColor();

            return DrawText.Create("").Color(color)
                .Append($"[{Game.Strings["GameTexts"]["Judgment"]}]")
                .ResetColor().Append(".");
        }

        private DrawText BlessingText()
        {
            var color = IsAvailableByRank(4)
                ? Color.Orange
                : "#bda873".AsColor();

            return DrawText.Create("").Color(color)
                .Append($"[{Game.Strings["GameTexts"]["Blessing"]}]")
                .ResetColor().Append(".");
        }

        private bool IsAvailableByRank(int rank)
        {
            return _prayerAbility.AbilityRank.Value >= rank;
        }

        private void Panel_TouchDown(object sender, MyraEventArgs e)
        {
            this.Close();
        }

        private void SetDescription(Gods god)
        {
        }

        private void Select(Image img, Gods? god, bool isSelection = false)
        {
            if (god == null)
            {
                if (_selected == default)
                {
                    _godName.Text = "";
                    img.Color = Color.White;
                }
                else
                {
                    img.Color = Color.White;
                    Select(_selectedImage, _selected);
                }
                return;
            }

            if (isSelection)
            {
                _selected = god;
                _acceptBtn.Enabled = true;

                if (_selectedImage != null)
                    _selectedImage.Color = Color.White;

                _selectedImage = img;
            }

            img.Color = Color.Gray;
            _godName.Text = Game.Strings["GodNames"][god.ToString()];

            UpdJudjement(god);
            UpdBlessing(god);
        }

        public override void Close()
        {
            _bloom.Disable();
            base.Close();
        }

        private class GodImage
        {
            public GodImage(Texture2D texture, Point position)
            {
                Bounds = new Rectangle()
                {
                    Height = 128,
                    Width = 128,
                    X = position.X,
                    Y = position.Y
                };
                Texture = texture;
            }

            public Color Color { get; set; }

            public Texture2D Texture { get; set; }

            public Rectangle Bounds { get; set; }
        }
    }
}