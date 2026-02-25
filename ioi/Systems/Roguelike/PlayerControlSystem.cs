using Geranium.Reflection;
using ioi.Components;
using ioi.Struct;
using ioi.Widgets.UserInterfaces.Roguelike;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MoonSharp.Interpreter;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class PlayerControlSystem : IDisposable
    {
        public GameHost Game { get; private set; }

        public Mode Mode { get; set; }

        public ControlsWidget ControlsWidget { get; internal set; }

        private TimeSpan _total;

        public PlayerControlSystem(GameHost game)
        {
            Game = game;
        }

        public void Update(GameTime gameTime)
        {
            switch (Mode)
            {
                case Mode.Map:
                    UpdateMap(gameTime);
                    break;
                case Mode.Combat:
                    UpdateCombat(gameTime);
                    break;
                default:
                    break;
            }
        }

        public void UpdateCombat(GameTime gameTime)
        {
            var key = KeyboardExtended.GetState();

            var player = Game.GameState.Player.Entity;
            var enemy = Game.GameState.Enemy;
            
            if (key.WasKeyPressed(Keys.D1))
            {

            }

            if (key.WasKeyPressed(Keys.D2))
            {

            }

            if (key.WasKeyPressed(Keys.D3))
            {

            }

            if (key.WasKeyPressed(Keys.D4))
            {

            }

            if (key.WasKeyPressed(Keys.I))
            {

            }

            if (key.WasKeyPressed(Keys.A))
            {
                Game.GameWorld.CombatSystem.Attack(player, enemy);
            }

            if (key.WasKeyPressed(Keys.D))
            {

            }

            if (key.WasKeyPressed(Keys.F))
            {

            }

            if (key.WasKeyPressed(Keys.Q))
            {

            }
        }

        public void UpdateMap(GameTime gameTime)
        {
            var player = Game.GameState.Player;

            MouseMoving(player);

            if (player.IsMoving)
            {
                var diff = player.UpdateMoving();

                CameraMoving(player, diff);
                return;
            }

            if (this.CanUpdate(gameTime, TimeSpan.FromSeconds(1)))
            {
                Game.GameState.Temp.ClickPosition = null;
            }

            if (player.MovePath != default)
            {
                var target = player.MovePath.Dequeue();
                var moving = Game.GameState.Map.Move(player, target);
                if (!moving)
                    return;

                if (player.MovePath.Count == 0)
                {
                    player.MovePath = null;
                    Game.GameState.Temp.ClickPosition = null;
                }
            }

            if (!player.IsMoving)
                KeyboardMoving(player);
        }

        private void MouseMoving(ObjectMap player)
        {
            var state = MouseExtended.GetState();

            if (state.WasButtonPressed(MouseButton.Left))
            {
                var pos = state.Position;
                var clicked = Game.CameraMap.ScreenToWorld(pos.X, pos.Y);
                var targetCoords = new Microsoft.Xna.Framework.Point(((int)Math.Floor(clicked.X / Game.CellSize.X)), ((int)Math.Floor(clicked.Y / Game.CellSize.Y)));

                Queue<Point> path = default;

                var isClickOutOfMap = targetCoords.X > Game.GameState.Map.Width || targetCoords.Y > Game.GameState.Map.Height;
                if (!isClickOutOfMap)
                {
                    path = player.BindMovePath(targetCoords);
                }

                if (Game.MapViewport.Bounds.Contains(pos))
                {
                    Game.GameState.Temp.ClickPosition = clicked;
                    Game.GameState.Temp.ClickSprite = path == default ? Entities.Data.Temporary.ClickSprite.Cross : Entities.Data.Temporary.ClickSprite.Position;
                }
            }
        }

        private void KeyboardMoving(Components.ObjectMap player)
        {
            var keyboard = KeyboardExtended.GetState();

            int x = 0, y = 0;

            if (keyboard.IsKeyDown(Keys.S))
            {
                y += 1;
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                y -= 1;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                x -= 1;
                player.Side = Side.Left;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                x += 1;
                player.Side = Side.Right;
            }

            var coords = new Point(x, y);

            if (coords == Point.Zero)
                return;

            coords = player.Coords + coords;

            Game.GameState.Map.Move(player, coords);
        }

        private void CameraMoving(Components.ObjectMap player, Vector2 diffMove)
        {
            BoundingBox deadzoneBounds = GetDeadzoneBounds();

            var playerBounds = player.BoundingBox;

            Vector2 cameraAdjustment = Vector2.Zero;

            bool lookat = false;

            // x
            if (playerBounds.Max.X < deadzoneBounds.Min.X || playerBounds.Min.X > deadzoneBounds.Max.X)
            {
                cameraAdjustment.X = diffMove.X;
            }

            // y
            if (playerBounds.Max.Y < deadzoneBounds.Min.Y || playerBounds.Min.Y > deadzoneBounds.Max.Y)
            {
                cameraAdjustment.Y = diffMove.Y;
            }

            if(deadzoneBounds.Contains(player.BoundingBoxCamera)== ContainmentType.Disjoint)
            {
                Game.CameraMap.LookAt(player.Position);
                return;
            }


            if (cameraAdjustment != Vector2.Zero)
            {
                Game.CameraMap.Move(cameraAdjustment);
            }
        }

        private BoundingBox GetDeadzoneBounds()
        {
            var viewport = Game.CameraMap.BoundingRectangle;

            float deadzoneWidth = viewport.Width * 0.75f;
            float deadzoneHeight = viewport.Height * 0.75f;

            var diff = new Vector2(viewport.Width - deadzoneWidth, viewport.Height - deadzoneHeight) / 2;

            var deadzoneX = Game.CameraMap.BoundingRectangle.X + diff.X;
            var deadzoneY = Game.CameraMap.BoundingRectangle.Y + diff.Y;

            var max = new Vector3(deadzoneX + deadzoneWidth, deadzoneY + deadzoneHeight, 0.5f);
            var min = new Vector3(deadzoneX, deadzoneY, 0.5f);

            var deadzoneBounds = new BoundingBox(min, max);
            return deadzoneBounds;
        }

        public void Draw(GameTime gameTime)
        {
            if (!Game.IsDrawBounds || Mode == Mode.Combat)
                return;

            var player = Game.GameState.Player;

            BoundingBox deadzoneBounds = GetDeadzoneBounds();

            var sb = Game.BeginDraw(camera: Game.CameraMap);

            sb.DrawRectangle(new RectangleF(deadzoneBounds.Min.X, deadzoneBounds.Min.Y, 
                deadzoneBounds.Max.X- deadzoneBounds.Min.X, 
                deadzoneBounds.Max.Y- deadzoneBounds.Min.Y), Color.Yellow, 2);

            sb.DrawRectangle(new RectangleF(player.BoundingBox.Min.X, player.BoundingBox.Min.Y, 
                player.BoundingBox.Max.X- player.BoundingBox.Min.X, 
                player.BoundingBox.Max.Y- player.BoundingBox.Min.Y), Color.Red, 2);

            sb.DrawRectangle(new RectangleF(player.BoundingBoxCamera.Min.X, player.BoundingBoxCamera.Min.Y,
                player.BoundingBoxCamera.Max.X - player.BoundingBoxCamera.Min.X,
                player.BoundingBoxCamera.Max.Y - player.BoundingBoxCamera.Min.Y), Color.LightYellow, 2);
            sb.End();
        }

        public void ControlsMainScreenPreset()
        {
            var str = Game.Strings["Roguelike"];
            ControlsWidget.BindButton(1, $"[Q/MRB] - {str["info"]}");
            ControlsWidget.BindButton(2, $"[W,A,S,D/LMB] - {str["controlwasd"]}");
            ControlsWidget.BindButton(3, $"[1,2,3,4] - {str["abils"]}");
            ControlsWidget.BindButton(4, $"[E] - {str["controluse"]}");
            ControlsWidget.BindButton(5, $"[5,6,7,8] - {str["skills"]}");
            ControlsWidget.BindButton(6, $"[С] - {str["charinfo"]}");
            ControlsWidget.BindButton(7, $"[M] - {str["map"]}");
            ControlsWidget.BindButton(8, $"[I] - {str["inventory"]}");
            ControlsWidget.BindButton(9, $"[F1-F6] - {str["charselectcontrol"]}");
            ControlsWidget.BindButton(10, $"[<,^,>] - {str["camera"]}");
        }

        public void ControlsCombatPreset()
        {
            var entity = Game.GameState.Player.Entity;

            var str = Game.Strings["Roguelike"];
            ControlsWidget.BindButton(1, $"[1] - {entity.GetAbilityName(1)}");
            ControlsWidget.BindButton(2, $"[2] - {entity.GetAbilityName(2)}");
            ControlsWidget.BindButton(3, $"[3] - {entity.GetAbilityName(3)}");
            ControlsWidget.BindButton(4, $"[4] - {entity.GetAbilityName(4)}");

            ControlsWidget.BindButton(5, $"[I] - {str["inventory"]}");

            ControlsWidget.BindButton(6, $"[A] - {str["doattack"]}");
            ControlsWidget.BindButton(7, $"[D] - {str["dodefence"]}");
            ControlsWidget.BindButton(8, $"[S] - {str["doflee"]}");

            ControlsWidget.BindButton(9, $"[F] - {str["dowait"]}");
            ControlsWidget.BindButton(10, $"[Q] - {str["info"]}");
        }

        public void Dispose()
        {
            Game = null;
            ControlsWidget?.Dispose();
            ControlsWidget = null;
        }

        internal void OnCollide(ObjectMap map)
        {
        }

        internal void Combat()
        {
            Mode = Mode.Combat;
        }

        internal void Map()
        {
            Mode = Mode.Map;
        }
    }
}
