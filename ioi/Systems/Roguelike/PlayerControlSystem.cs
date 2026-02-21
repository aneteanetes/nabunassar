using Geranium.Reflection;
using ioi.Components;
using ioi.Struct;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike
{
    internal class PlayerControlSystem
    {
        public GameHost Game { get; }
        private TimeSpan _total;

        public PlayerControlSystem(GameHost game)
        {
            Game = game;
        }

        public void Update(GameTime gameTime)
        {
            var player = Game.GameState.Player.MapObject;

            MouseMoving(player);

            if (player.IsMoving)
            {
                var newPos = Vector2.Lerp(player.Position, player.TargetPosition, 1f);
                var diff = newPos - player.Position;
                player.Position = newPos;
                if (Vector2.Distance(player.Position, player.TargetPosition) < 0.1f)
                {
                    player.Position = player.TargetPosition;
                    player.IsMoving = false;
                }

                CameraMoving(player, diff);
                return;
            }

            if (this.CanUpdate(gameTime, TimeSpan.FromSeconds(1)))
            {
                Game.GameState.Temp.ClickPosition = null;
            }

            if (player.MovingPath != default)
            {
                var target = player.MovingPath.Dequeue();
                Game.GameState.Map.Move(player, target);

                if (player.MovingPath.Count == 0)
                {
                    player.MovingPath = null;
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
                var clicked = Game.CameraMap.ScreenToWorld(pos.X,pos.Y);
                var targetCoords = new Vector2(((float)Math.Floor(clicked.X / Game.CellSize.X)), ((float)Math.Floor(clicked.Y / Game.CellSize.Y)));

                if (targetCoords.X > 90 || targetCoords.Y > 23)
                    return;

                var path = Game.PathfindSystem.FindPath(player.Coords, targetCoords);

                if (path != default)
                {
                    player.MovingPath = new Queue<Vector2>(path);
                }

                Game.GameState.Temp.ClickPosition = clicked;
                Game.GameState.Temp.ClickSprite = path == default ? Entities.Data.Temporary.ClickSprite.Cross : Entities.Data.Temporary.ClickSprite.Position;
            }
        }

        private void KeyboardMoving(Components.ObjectMap player)
        {
            var keyboard = KeyboardExtended.GetState();

            float x = 0, y = 0;

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

            var coords = new Vector2(x, y);

            if (coords == Vector2.Zero)
                return;

            coords = player.Coords + coords;

            var objs = Game.GameState.Map.Collide(coords);

            if (objs.Count > 0)
            {
                player.ProcessCollision(objs);
            }

            if (objs.Any(x => x.IsBounds))
                return;

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
            if (!Game.IsDrawBounds)
                return;

            var player = Game.GameState.Player.MapObject;

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
    }
}
