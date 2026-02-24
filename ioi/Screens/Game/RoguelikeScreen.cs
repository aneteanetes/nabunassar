using Geranium.Reflection;
using ioi.Entities.Struct;
using ioi.Monogame.SpriteBatch;
using ioi.Screens.Abstract;
using ioi.Tiled.Map;
using ioi.Widgets.UserInterfaces.Roguelike;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;

namespace ioi.Screens
{
    internal class RoguelikeScreen : BaseGameScreen
    {
        public RoguelikeScreen(GameHost game) : base(game) { }

        private Sprite positionSprite;
        private Sprite crossSprite;

        public Effect Celshading { get; private set; }

        public ControlsWidget ControlsWidget { get; set; }

        public override void LoadContent()
        {
            Celshading = Game.Content.Load<Effect>("Assets/Shaders/Celshading.fx");

            var cursorTileset = Game.Content.LoadTexture("Assets/Tilesets/cursor_tilemap_packed.png");
            positionSprite = new Sprite(new Texture2DRegion(cursorTileset, 272, 32, 16, 16)) { Color= Color.AntiqueWhite };
            crossSprite = new Sprite(new Texture2DRegion(cursorTileset, 272, 0, 16, 16)) { Color = Color.Red, };

            Game.AddDesktopWidget(new EntityWidget(Game,Game.GameState.Player, Struct.Side.Right),Game.MyraDesktopIngame);
            Game.GameWorld.PlayerControlSystem.ControlsWidget = Game.AddDesktopWidget(new ControlsWidget(Game),Game.MyraDesktopIngame);
            Game.GameWorld.PlayerControlSystem.MainScreenPreset();

            Game.GameWorld.LogSystem.Widget = Game.AddDesktopWidget(new LogWidget(Game), Game.MyraDesktopIngame);
            Game.GameWorld.MapSystem.LogArea();

            Game.GameWorld.MapSystem.Celshading = Celshading;

            Game.GameWorld.BorderLayersSystem["Controls"] = true;
            Game.GameWorld.BorderLayersSystem["Player"] = true;
            Game.GameWorld.BorderLayersSystem["Skills"] = true;
            Game.GameWorld.BorderLayersSystem["Map"] = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (GameHost.IsMakingScreenShot == false && Game.LastScreenshot.IsNotEmpty())
            {
                Game.GameWorld.LogSystem.Log(DrawText.Create(Game.Strings["Roguelike"]["screenshotsaved"], Color.DarkGray).AppendSpace().Append(Game.LastScreenshot));
                Game.LastScreenshot = default;
            }

            GameController.GlobalMenuWidget();
            
            Game.GameWorld.Update(gameTime);

            var keystate = KeyboardExtended.GetState();
            if (keystate.IsControlDown() && keystate.WasKeyPressed(Keys.S))
            {
                GameHost.Game.MakeScreenshot();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(12, 12, 12));

            base.Draw(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            Game.GameWorld.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.Penumbra.Draw(gameTime);

            Game.SpriteBatch.End();

            if (Game.GameState.Temp.ClickPosition.HasValue)
            {
                var sb = Game.BeginDraw(effect: Celshading, camera: Game.CameraMap);

                var sprite = Game.GameState.Temp.ClickSprite == Entities.Data.Temporary.ClickSprite.Position
                    ? positionSprite
                    : crossSprite;

                var drawPos = Game.GameState.Temp.ClickPosition.Value;

                sb.Draw(sprite, drawPos, 0, new Vector2(2, 2));

                sb.End();
            }
        }

        public override void UnloadContent()
        {
            Game.RemoveDesktopWidgets(true);
            Game.RemoveDesktopWidgets(true,Game.MyraDesktopIngame);
        }

        public override void Dispose()
        {
            Game.GameWorld.Dispose();
        }
    }
}