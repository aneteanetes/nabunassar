using FontStashSharp;
using Geranium.Reflection;
using ioi.Components;
using ioi.Widgets.UserInterfaces.Roguelike;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using MoonSharp.Interpreter;
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
        private bool _endOfBattle;
        private object battleScreenLock = new();

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
            if (_endOfBattle)
            {
                if (this.CanUpdate(gameTime, TimeSpan.FromSeconds(1), battleScreenLock))
                    CloseCombat();
            }
            else if (enemyWidget != default)
            {
                var enemy = enemyWidget.EntityFetcher?.Invoke();
                if (enemy == null)
                    return;

                var currentPlayer = Game.GameState.Player.Entity.Squad.FirstAlive();
                combatHeader.Text = $"{enemy["hp"]}/{enemy["mhp"]} {enemy.GetName()} VS {currentPlayer.GetName()} {currentPlayer["hp"]}/{currentPlayer["mhp"]}";
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
                yield return 0;
            }

            Game.World.LoadingSystem.LoadCenter(loading(), GetLoadDelay(), changeScreenBack());
        }

        public void EndCombat()
        {
            Game.World.PlayerControlSystem.Disable();
            _endOfBattle = true;
        }

        public void CloseCombat()
        {
            _endOfBattle = false;
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
            foreach (var near in nearest)
            {
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

        public void Dispose()
        {
            Game.MyraDesktopIngame.Widgets.Remove(headerPanel);
            Game.RemoveDesktopWidget(log, Game.MyraDesktopIngame);
            Game.RemoveDesktopWidget(enemyWidget, Game.MyraDesktopIngame);
        }

        private bool isGameOver = false;
        public void Kill(GameEntity entity)
        {
            var player = Game.GameState.Player;
            if (entity["type"].String== "player")
            {
                player.Entity.Unconscious();
                if (player.Entity.Squad.IsAnybodyAlive())
                {
                    player.Entity.Squad.MoveNextAlive();
                    player.BindEntity(player.Entity.Squad.Current);
                }
                else
                {
                    isGameOver = true;
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

            if (isGameOver)
            {
                Game.GameOver();
                return;
            }

            player.Squad.MoveNextAlive();
            Game.GameState.Player.BindEntity(player.Squad.Current);
        }

        internal void Strike(GameEntity player, GameEntity enemy)
        {
            player.Func("strike", enemy);
            Turn(player.Squad.Leader, enemy);
        }

        internal void Defence(GameEntity player, GameEntity enemy)
        {
            player.Func("defence", enemy);
            Turn(player.Squad.Leader, enemy);
        }

        internal void Flee(GameEntity player, GameEntity enemy)
        {
            player.Func("flee", enemy);
            Turn(player.Squad.Leader, enemy);
        }
    }
}