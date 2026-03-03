using FontStashSharp;
using FontStashSharp.RichText;
using Geranium.Reflection;
using ioi.Components;
using ioi.Widgets.UserInterfaces.Roguelike;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using MoonSharp.Interpreter;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    public class CombatSystem : IDisposable
    {
        private GameHost Game { get; }
        public bool IsInCombat { get; internal set; }

        EntityWidget enemyWidget;
        private FontSystem consolas;
        private Panel headerPanel;
        private Label combatHeader;
        private CombatLogWidget log;
        public bool IsEndOfBattle { get; private set;  }
        private object battleScreenLock = new();
        private int _round;
        private bool _delayAfterEnd;

        internal CombatSystem(GameHost game)
        {
            Game = game;
        }

        public void LoadContent()
        {
            consolas = Game.Content.LoadFont(Fonts.Consolas);

            headerPanel = new Panel()
            {
                Width = 964,
                Height = 52,
                Left = 478,
                Top = 24,
                Visible = false
            };
            combatHeader = new Label()
            {
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = consolas.GetFont(26),
                TextColor = Color.Gray,
            };
            headerPanel.Widgets.Add(combatHeader);
            Game.MyraDesktopIngame.Widgets.Add(headerPanel);

            log = new CombatLogWidget(Game)
            {
                Visible = false
            };
            Game.AddDesktopWidget(log, Game.MyraDesktopIngame);
        }

        public void Update(GameTime gameTime)
        {
            if (IsEndOfBattle)
            {
                if (this.CanUpdate(gameTime, TimeSpan.FromSeconds(1), battleScreenLock))
                    _delayAfterEnd = true;

                if (_delayAfterEnd)
                {
                    var keyCount = KeyboardExtended.GetState().GetPressedKeyCount;
                    if (keyCount > 0)
                        CloseCombat();
                }
            }
            else if (enemyWidget != default)
            {
                var enemy = enemyWidget.EntityFetcher?.Invoke();
                if (enemy == null)
                    return;

                var currentPlayer = Game.GameState.Player.Entity.Squad.FirstAlive();
                if (currentPlayer == null)
                {
                    combatHeader.Text = Game.Strings["Roguelike"]["GameOver"];
                }
                else
                {
                    combatHeader.Text = $"{enemy["hp"]}/{enemy["mhp"]} {enemy.GetName()} VS {currentPlayer.GetName()} {currentPlayer["hp"]}/{currentPlayer["mhp"]}";
                }
            }
        }

        public void StartCombat(GameEntity enemy)
        {
            Game.World.MapSystem.Pause();
            IsInCombat = true;

            AssemblySquad(enemy);

            IEnumerator loading()
            {
                var str = Game.Strings["Roguelike"];
                headerPanel.Visible = true;
                combatHeader.Text = $"{str[enemy["name"].String]} VS {Game.GameState.Player.Entity.Name}";
                Game.World.BorderLayersSystem["Map"] = true;
                Game.World.PlayerControlSystem.Combat();
                yield return 0;

                BindCurrentEnemy(enemy);

                yield return 1;
            }

            IEnumerator changeScreenBack()
            {
                Game.World.BorderLayersSystem["Map"] = false;
                Game.World.BorderLayersSystem["LeftPanel"] = true;
                Game.World.BorderLayersSystem["Center"] = true;
                Game.World.PlayerControlSystem.ControlsCombatPreset();
                _round = 0;
                yield return 0;
            }

            Game.World.LoadingSystem.LoadCenter(loading(), GetLoadDelay(), changeScreenBack());
        }

        public void EndCombat()
        {
            Game.World.PlayerControlSystem.Disable();
            IsEndOfBattle = true;

            LogCombatDelimiter();
            LogCombat(Game.Strings["Roguelike"]["battleoverkeypress"]);
        }

        public void CloseCombat()
        {
            if (IsGameOver)
            {
                Game.GameOver();
                return;
            }

            IsEndOfBattle = false;
            _delayAfterEnd = false;
            IEnumerator loading()
            {
                log.Visible = false;
                log.Clear();
                headerPanel.Visible = false;
                Game.World.BorderLayersSystem["LeftPanel"] = false;
                Game.World.BorderLayersSystem["Center"] = false;
                Game.World.BorderLayersSystem["Map"] = true;
                Game.World.PlayerControlSystem.Map();

                //if (enemyWidget.Entity["hp"].Number <= 0)
                //    enemyWidget.Entity.Destroy();

                Game.RemoveDesktopWidget(enemyWidget, Game.MyraDesktopIngame);
                yield return 0;
            }

            IEnumerator afterLoad()
            {
                Game.World.PlayerControlSystem.ControlsMainScreenPreset();
                Game.World.PlayerControlSystem.Enable();
                IsInCombat = false;
                Game.World.MapSystem.Resume();
                yield return 0;
            }

            Game.World.LoadingSystem.LoadCenter(loading(), GetLoadDelay(), afterLoad());
        }

        private void AssemblySquad(GameEntity enemy)
        {
            var nearest = Game.World.MapSystem.CollectNearest(enemy);
            if (enemy.Squad == null)
            {
                enemy.Squad=new GameEntitySquad(enemy);
            }
            foreach (var near in nearest)
            {
                if (near.Squad != default)
                    near.Squad.Destroy();
                enemy.Squad.Add(near);
            }
        }

        private void BindCurrentEnemy(GameEntity enemy)
        {
            if (enemyWidget != null)
                Game.RemoveDesktopWidget(enemyWidget, Game.MyraDesktopIngame);

            Game.GameState.Enemy = enemy;
            enemyWidget = Game.AddDesktopWidget(new EntityWidget(Game, null, Struct.Side.Left,enemy), Game.MyraDesktopIngame);
        }

        private int GetLoadDelay()
        {
            var delay = Game.Lua.Globals["Core"].As<Table>()["combatDelayMS"].As<double>();
            return ((int)delay);
        }

        public void LogCombat(string text)
        {
            log.AppendLine(text);
        }
        public void LogCombatDelimiter()
        {
            log.AppendDelimiter();
        }

        public void Dispose()
        {
            Game.MyraDesktopIngame.Widgets.Remove(headerPanel);
            Game.RemoveDesktopWidget(log, Game.MyraDesktopIngame);
            Game.RemoveDesktopWidget(enemyWidget, Game.MyraDesktopIngame);
        }

        public bool IsGameOver { get; private set; }

        public void Kill(GameEntity entity)
        {
            var player = Game.GameState.Player;
            if (entity["type"].String== "player")
            {
                /// т.к. враги атакуют первого персонажа, он может быть уже мёртв когда его атакуют следующие
                /// в таком случае мы уже сделали всё что нужно, и единственное что осталось это
                /// уменьшить кол-во хп у персонажа которого атаковали, чтобы потом определить его
                /// "степень невменяемости" на основе текущего хп
                if (entity.IsUnconscious && player.Entity.Squad.Current != entity)
                    return;

                entity.Unconscious();
                if (entity.Squad.IsAnybodyAlive())
                {
                    entity.Squad.MoveNextAlive();
                    player.BindEntity(entity.Squad.Current);
                }
                else
                {
                    IsGameOver = true;
                }
            }
            else
            {
                var squad = entity.Squad;
                squad.Remove(entity);

                if(squad.IsEmpty())
                {
                    EndCombat();
                }
                else
                {
                    squad.MoveNext();
                    BindCurrentEnemy(squad.Current);
                }
            }
        }

        public void Turn(GameEntity player, GameEntity enemy)
        {
            enemy.Squad?.CombatTurn(player);

            if (IsGameOver)
            {
                EndCombat();
                return;
            }

            player.Squad.MoveNextAlive();
            Game.GameState.Player.BindEntity(player.Squad.Current);
        }

        internal void Strike(GameEntity player, GameEntity enemy)
        {
            Game.World.CombatSystem.AppendRound();
            player.Func("strike", enemy);
            Turn(player.Squad.Leader, enemy);
        }

        internal void Defence(GameEntity player, GameEntity enemy)
        {
            Game.World.CombatSystem.AppendRound();
            player.Func("defence", enemy);
            Turn(player.Squad.Leader, enemy);
        }

        internal void Flee(GameEntity player, GameEntity enemy)
        {
            Game.World.CombatSystem.AppendRound();
            player.Func("flee", enemy);
            Turn(player.Squad.Leader, enemy);
        }

        public void AppendRound()
        {
            _round++;
            log.AppendDelimiterLine(_round);
        }

        internal void UseAbility(GameEntity player, int slot, GameEntity enemy)
        {
            var abilityVal = player.Func("getAbility", slot);

            if (abilityVal.IsNil())
                return;

            var entity = abilityVal.UserData.Object.As<GameEntity>();

            var ability = player.Func($"ability{slot}");

            var abilcolor = $"/c[{entity.Color("color").ToHexString()}]";
            var abname = entity.GetName();

            if (entity["mode"].String == "passive")
            {
                Game.World.LogSystem.Log($"{Game.Strings["Roguelike"]["passiveab"]} {Game.Strings["Roguelike"]["ability"].ToLower()} '{abilcolor}{abname}' /cd{Game.Strings["Roguelike"]["cantuse"]}!");
                return;
            }

            if (entity["location"].String != "combat")
            {
                Game.World.LogSystem.Log($"{Game.Strings["Roguelike"]["ability"]} '{abilcolor}{abname}' /cd{Game.Strings["Roguelike"]["cantuseincombat"]}!");
                return;
            }

            var canCast = entity.Func("canCast", player, enemy).Boolean;
            if (canCast)
            {
                Game.World.CombatSystem.AppendRound();
                entity.Func("cast", player, enemy);
                Turn(player.Squad.Leader, enemy);
            }
            else
            {
                Game.World.LogSystem.Log($"{player.GetNameColored()} /cd{Game.Strings["Roguelike"]["cantuseabil"]} {abilcolor}{abname}/cd!");
            }
        }
    }
}