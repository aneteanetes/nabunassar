using FontStashSharp.RichText;
using Geranium.Reflection;
using ioi.Components;
using ioi.Monogame.Settings;
using ioi.Struct;
using ioi.Systems.Roguelike.Controllings;
using ioi.Widgets.UserInterfaces.Roguelike;
using ioi.Widgets.UserInterfaces.Roguelike.CharacterInfo;
using ioi.Widgets.UserInterfaces.Roguelike.InfoList;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled;
using MoonSharp.Interpreter;
using Myra.Graphics2D.TextureAtlases;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class PlayerControlSystem : IDisposable
    {
        private InfoWidget infoList;

        public GameHost Game { get; private set; }

        public Mode Mode { get; set; }

        public ControlsWidget ControlsWidget { get; internal set; }
        public bool Enabled { get; private set; } = true;

        public PlayerControlSystem(GameHost game)
        {
            Game = game;
        }

        public void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            switch (Mode)
            {
                case Mode.Map:
                    UpdateMap(gameTime);
                    break;
                case Mode.Combat:
                    UpdateCombat(gameTime);
                    break;
                case Mode.Info:
                    UpdateInfo(gameTime);
                    break;
                case Mode.Inventory:
                    UpdateInventory(gameTime);
                    break;
                default:
                    break;
            }
        }

        public void UpdateInventory(GameTime gameTime)
        {
            var playerMap = Game.GameState.Player;
            var player = playerMap.Entity;

            var controls = GetControls();
            if (controls.Back.WasPressed())
            {
                CloseInventory();
            }
        }

        public void UpdateInfo(GameTime gameTime)
        {
            var playerMap = Game.GameState.Player;
            var player = playerMap.Entity;

            var controls = GetControls();
            if (controls.Back.WasPressed())
            {
                CloseInfoList();
                MapMode();
            }

            if (controls.ListUp.WasPressed())
            {
                infoList.Up();
            }

            if (controls.ListDown.WasPressed())
            {
                infoList.Down();
            }

            if (controls.ListTakeAll.WasPressed())
            {
                var coords = Game.GameState.Player.Coords;
                var cell = Game.GameState.Map[coords];

                var itemMaps = cell.Objects
                    .Where(obj => (obj.Entity?["type"]?.String ?? "") == "item")
                    .ToArray();

                foreach (var itemMap in itemMaps)
                {
                    itemMap.RemoveFromMap();
                    player.TakeItems(itemMap.Entity);
                }

                infoList.Refresh(cell.Objects);

                if (infoList.IsEmpty())
                {
                    CloseInfoList();
                    MapMode();
                }
            }
        }

        public void UpdateCombat(GameTime gameTime)
        {
            GameController.GlobalMenuWidget();

            var key = KeyboardExtended.GetState();

            var player = Game.GameState.Player.Entity;
            var enemy = Game.GameState.Enemy;

            UpdateAbilityPreset();

            bool isAction = false;
            bool wasKeyPressed(Keys keys)
            {
                var isPressed = key.WasKeyPressed(keys);
                if (isPressed)
                {
                    isAction = true;
                }
                return isPressed;
            }

            if (wasKeyPressed(Keys.D1))
            {
                Game.World.CombatSystem.UseAbility(player,1, enemy);
            }

            if (wasKeyPressed(Keys.D2))
            {
                Game.World.CombatSystem.UseAbility(player, 2, enemy);
            }

            if (wasKeyPressed(Keys.D3))
            {
                Game.World.CombatSystem.UseAbility(player, 3, enemy);
            }

            if (wasKeyPressed(Keys.D4))
            {
                Game.World.CombatSystem.UseAbility(player, 4, enemy);
            }

            if (wasKeyPressed(Keys.I))
            {
            }

            if (wasKeyPressed(Keys.A))
            {
                Game.World.CombatSystem.Strike(player, enemy);
            }

            if (wasKeyPressed(Keys.D))
            {
                Game.World.CombatSystem.Defence(player, enemy);
            }

            if(wasKeyPressed(Keys.S))
            {
                Game.World.CombatSystem.Flee(player, enemy);
            }

            if (wasKeyPressed(Keys.F))
            {

            }

            if (key.WasKeyPressed(Keys.Q))
            {

            }

            if (isAction)
            {
                player.Func("tick");
            }
        }

        public void UpdateMap(GameTime gameTime)
        {
            var player = Game.GameState.Player;
            var controls = GetControls();

            GameController.GlobalMenuWidget();

            Game.CameraMap.Move(GetMovementDirection());

            #region moving

            MouseMoving(player, controls);

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

            #endregion

            void wasPartySelectPressed(ControlSchemeKey fKey, int idx)
            {
                if (fKey.WasPressed())
                {
                    if (player.Entity.Squad.TrySetMember(idx - 1, out var member))
                    {
                        player.BindEntity(member);
                    }
                    else if (member!=null)
                    {
                        Game.World.LogSystem.Log($"{member.GetNameColored()} /cd{Game.Strings["Roguelike"]["inunconscous"]}!");
                    }
                }
            }

            wasPartySelectPressed(controls.Party1,1);
            wasPartySelectPressed(controls.Party2,2);
            wasPartySelectPressed(controls.Party3,3);
            wasPartySelectPressed(controls.Party4,4);
            wasPartySelectPressed(controls.Party5,5);
            wasPartySelectPressed(controls.Party6,6);

            if (controls.Info.WasPressed())
            {
                bool isOpened = OpenCellInfo(player);
                if (isOpened)
                {
                    return;
                }
            }

            if (controls.Inventory.WasPressed())
            {
                InventoryMode();
            }

            if (!player.IsMoving)
                KeyboardMoving(player,controls);
        }

        private bool OpenCellInfo(ObjectMap obj)
        {
            var cell = Game.World.MapSystem.GetCell(obj.Coords.X, obj.Coords.Y);
            var objs = cell.Objects.Except([obj]).ToArray();
            if (objs.Length > 0)
            {
                this.InfoMode();
                Game.World.BorderSystem["LeftPanel"] = true;
                infoList = Game.AddDesktopWidget(new InfoWidget(Game, objs), Game.MyraDesktopIngame);

                PresetInfoList();

                return true;
            }

            return false;
        }

        private void CloseInfoList()
        {
            Game.World.BorderSystem["LeftPanel"] = false;
            Game.RemoveDesktopWidgets<InfoWidget>(0, Game.MyraDesktopIngame);
        }

        private void MouseMoving(ObjectMap player, ControlScheme controls)
        {
            if (controls.MouseLeftButton.WasPressed())
            {
                var pos = MouseExtended.GetState().Position;
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

        private void KeyboardMoving(ObjectMap player, ControlScheme controls)
        {
            int x = 0, y = 0;

            if (controls.MoveDown.IsDown())
            {
                y += 1;
            }
            if (controls.MoveUp.IsDown())
            {
                y -= 1;
            }
            if (controls.MoveLeft.IsDown())
            {
                x -= 1;
                player.Side = Side.Left;
            }
            if (controls.MoveRight.IsDown())
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

        private void CameraMoving(ObjectMap player, Vector2 diffMove)
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

        public void PresetMap()
        {
            ControlsWidget.Reset();

            var controls = GetControls();

            var str = Game.Strings["Roguelike"];

            ControlsWidget.BindButtonKey(1, str["info"], "/", controls.Info, controls.MouseRightButton);
            ControlsWidget.BindButtonKey(2, str["controlwasd"], ",", controls.MoveUp, controls.MoveLeft, controls.MoveDown, controls.MoveRight, controls.MouseLeftButton);
            ControlsWidget.BindButtonKey(3, str["charselectcontrol"], "-", controls.Party1,controls.Party6);
            ControlsWidget.BindButtonKey(4, str["controluse"], "-", controls.Use);
            ControlsWidget.BindButtonKey(5, str["skills"], "-", controls.Ability1, controls.Skill4);
            ControlsWidget.BindButtonKey(6, str["doattack"], null, controls.Attack);
            ControlsWidget.BindButtonKey(7, str["charinfo"], null, controls.CharInfo);
            ControlsWidget.BindButtonKey(8, str["inventory"], null, controls.Inventory);
            ControlsWidget.BindButtonKey(9, str["map"], null, controls.Map);
            ControlsWidget.BindButtonKey(10, str["camera"], null, controls.Camera);
        }

        public void PresetCombat()
        {
            ControlsWidget.Reset();

            var controls = GetControls();
            var entity = Game.GameState.Player.Entity;

            var str = Game.Strings["Roguelike"];

            
            Dictionary<int, ControlSchemeKey> abils = new()
            {
                {1,controls.Ability1Combat },
                {2,controls.Ability2Combat },
                {3,controls.Ability3Combat },
                {4,controls.Ability4Combat },
            };
            foreach (var abilitem in abils)
            {
                var abil = entity.GetAbility(abilitem.Key);
                if (abil != null)
                {
                    var key = abilitem.Value;

                    if (abil["mode"].String == "passive")
                        key = null;

                    ControlsWidget.BindButtonKey(abilitem.Key, "", null, key);
                }
            }

            ControlsWidget.BindButtonKey(5, str["doflee"], null, controls.Flee);
            ControlsWidget.BindButtonKey(6, str["doattack"], null, controls.Attack);
            ControlsWidget.BindButtonKey(7, str["dodefence"], null, controls.Defence);
            ControlsWidget.BindButtonKey(8, str["inventory"], null, controls.Inventory);
            ControlsWidget.BindButtonKey(9, str["dowait"], null, controls.Waiting);
            ControlsWidget.BindButtonKey(10, str["info"], null, controls.Info);
        }

        public void PresetInfoList()
        {
            ControlsWidget.Reset();

            var controls = GetControls();

            var str = Game.Strings["Roguelike"];

            ControlsWidget.BindButtonKey(1, str["info"], null, controls.Info);
            ControlsWidget.BindButtonKey(2, str["up"], null, controls.ListUp);
            ControlsWidget.BindButtonKey(3, str["down"], null, controls.ListDown);
            ControlsWidget.BindButtonKey(4, str["options"], "/", controls.ListOptions, controls.MouseRightButton);
            ControlsWidget.BindButtonKey(5, str["close"], null, controls.CloseInfoList);

            ControlsWidget.BindButtonKey(6, str["doattack"], null, controls.Attack);
            ControlsWidget.BindButtonKey(7, $"{str["usage"]}", "/", controls.ListAction, controls.MouseLeftButton);
            ControlsWidget.BindButtonKey(8, str["takeall"], null, controls.ListTakeAll);
        }

        private void PresetInventory()
        {
            ControlsWidget.Reset();

            var controls = GetControls();

            var str = Game.Strings["Roguelike"];

            ControlsWidget.BindButtonKey(1, str["left"], null, controls.MenuLB);
            ControlsWidget.BindButtonKey(2, str["right"], null, controls.MenuRB);
            //ControlsWidget.BindButtonKey(3, str["down"], null, controls.ListDown);
            //ControlsWidget.BindButtonKey(4, str["options"], "/", controls.ListOptions, controls.MouseRightButton);
            ControlsWidget.BindButtonKey(5, str["close"], null, controls.CloseInfoList);

            //ControlsWidget.BindButtonKey(6, str["doattack"], null, controls.Attack);
            //ControlsWidget.BindButtonKey(7, $"{str["usage"]}", "/", controls.ListAction, controls.MouseLeftButton);
            //ControlsWidget.BindButtonKey(8, str["takeall"], null, controls.ListTakeAll);
        }

        private void UpdateAbilityPreset()
        {
            var controls = GetControls();
            var entity = Game.GameState.Player.Entity;

            for (int i = 1; i <= 4; i++)
            {
                var abil = entity.GetAbility(i);
                if (abil != null)
                {
                    var name = abil.GetName();
                    var rescolor = entity.Color("rescolor").ToHexString();
                    var cost = abil["cost"].Number;

                    var costtext = $" /c[{rescolor}][{cost}]";

                    if (abil["mode"].String == "passive")
                        costtext = string.Empty;

                    var nameRes = $"{name}{costtext}";

                    ControlsWidget.UpdateText(i, nameRes);
                }
                else
                {
                    ControlsWidget.BindButton(i, $" ");
                }
            }
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

        internal void CombatMode()
        {
            Mode = Mode.Combat;
            CloseInfoList();
        }

        internal void MapMode()
        {
            Mode = Mode.Map;
            this.PresetMap();
        }

        internal void InfoMode()
        {
            Mode = Mode.Info;
        }

        private void InventoryMode()
        {
            Game.World.MapSystem.Pause();
            Mode = Mode.Inventory;
            PresetInventory();

            Game.World.BorderSystem["Map"] = false;
            Game.World.BorderSystem["LeftPanel"] = true;
            Game.World.BorderSystem["Center"] = true;

            Game.AddDesktopWidget(new InventoryWidget(Game, Game.GameState.Player.Entity), Game.MyraDesktopIngame);

            var (header,label) = Game.World.BorderSystem.AddCenterHeader("Инвентарь");
            header.Visible = true;
        }

        public void CloseInventory()
        {
            Game.RemoveDesktopWidgets<InventoryWidget>(0, Game.MyraDesktopIngame);

            Game.World.BorderSystem.RemoveCenterHeader();
            Game.World.BorderSystem["LeftPanel"] = false;
            Game.World.BorderSystem["Center"] = false;

            Game.World.MapSystem.Resume();
            Game.World.BorderSystem["Map"] = true;
            Game.World.PlayerControlSystem.MapMode();
        }

        internal void Disable()
        {
            Enabled = false;
        }

        internal void Enable()
        {
            Enabled = true;
        }

        private Vector2 GetMovementDirection()
        {
            var controls = GetControls();
            Vector2 movementDirection = Vector2.Zero;

            if (controls.CameraDown.IsDown())
            {
                movementDirection += Vector2.UnitY;
            }

            if (controls.CameraUp.IsDown())
            {
                movementDirection -= Vector2.UnitY;
            }

            if (controls.CameraLeft.IsDown())
            {
                movementDirection -= Vector2.UnitX;
            }

            if (controls.CameraRight.IsDown())
            {
                movementDirection += Vector2.UnitX;
            }

            return movementDirection * 15;
        }

        public ControlScheme GetControls()
        {
            var schema = Game.Settings.ControlSchema;
            var controls = Game.Settings.Controls.FirstOrDefault(c => c.Schema == schema);
            if(controls == null)
            {
                controls = ControlScheme.Create(schema);
                Game.Settings.Controls.Add(controls);
            }

            return controls;
        }
    }
}
