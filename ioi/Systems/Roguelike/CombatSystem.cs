using FontStashSharp;
using Geranium.Reflection;
using ioi.Components;
using ioi.Widgets.UserInterfaces.Roguelike;
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
        }

        public void StartCombat(GameEntity enemy)
        {
            Game.GameWorld.MapSystem.Pause();
            IsInCombat = true;

            IEnumerator loading()
            {
                var str = Game.Strings["Roguelike"];
                headerPanel.Visible = true;
                combatHeader.Text = $"{str[enemy["name"].String]} VS {Game.GameState.Player.Entity.Name}";
                Game.GameWorld.BorderLayersSystem["Map"] = true;
                Game.GameWorld.PlayerControlSystem.Combat();
                enemyWidget = Game.AddDesktopWidget(new EntityWidget(Game, enemy, Struct.Side.Left), Game.MyraDesktopIngame);
                yield return 0;

                Game.GameState.Enemy = enemy;
                yield return 1;
            }

            IEnumerator changeScreenBack()
            {
                Game.GameWorld.BorderLayersSystem["Map"] = false;
                Game.GameWorld.BorderLayersSystem["LeftPanel"] = true;
                Game.GameWorld.BorderLayersSystem["Center"] = true;
                Game.GameWorld.PlayerControlSystem.ControlsCombatPreset();
                yield return 0;
            }

            Game.GameWorld.LoadingSystem.LoadCenter(loading(), GetLoadDelay(), changeScreenBack());
        }

        private int GetLoadDelay()
        {
            var delay = Game.Lua.Globals["Core"].As<Table>()["combatDelayMS"].As<double>();
            return ((int)delay);
        }

        public void CloseCombat()
        {
            _endOfBattle = false;
            IEnumerator loading()
            {
                log.Visible = false;
                log.Clear();
                headerPanel.Visible = false;
                Game.GameWorld.BorderLayersSystem["LeftPanel"] = false;
                Game.GameWorld.BorderLayersSystem["Center"] = false;
                Game.GameWorld.BorderLayersSystem["Map"] = true;
                Game.GameWorld.PlayerControlSystem.Map();

                if (enemyWidget.Entity["hp"].Number <= 0)
                    enemyWidget.Entity.Destroy();

                Game.RemoveDesktopWidget(enemyWidget, Game.MyraDesktopIngame);
                yield return 0;
            }

            IEnumerator afterLoad()
            {
                Game.GameWorld.PlayerControlSystem.ControlsMainScreenPreset();
                Game.GameWorld.PlayerControlSystem.Enable();
                IsInCombat = false;
                Game.GameWorld.MapSystem.Resume();
                yield return 0;
            }

            Game.GameWorld.LoadingSystem.LoadCenter(loading(), GetLoadDelay(), afterLoad());
        }

        public void Attack(GameEntity you, GameEntity enemy)
        {
            you.Func("strike", enemy.Data);
        }

        public void LogCombat(string text)
        {
            log.AppendLine(text);
        }

        public void Dispose()
        {
            Game.MyraDesktopIngame.Widgets.Remove(headerPanel);
        }

        public void EndCombat()
        {
            Game.GameWorld.PlayerControlSystem.Disable();
            _endOfBattle = true;
        }

        public void Kill(GameEntity entity)
        {
            if (Game.GameState.Player.Entity == entity)
            {
                //if player dies, stopping game
            }
            else // пока что бой только 1 на 1
            {
                EndCombat();
            }
        }
    }
}
