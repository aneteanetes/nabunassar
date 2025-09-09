using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Components.Effects;
using Nabunassar.Entities;
using Nabunassar.Monogame.Extended;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces;
using SharpFont.Cache;

namespace Nabunassar.Widgets.UserEffects
{
    internal class TeleportationInterface : ScreenWidget
    {
        private bool _isCaptured;
        private bool _isTeleporting;
        private Point _partyRectangleSize;
        private WavesShaderEffect _waves;
        private TeleportationShaderEffect _assemblyTP;
        private TeleportationShaderEffect _disassemblyTP;
        private Vector2 _partyRenderPosition;
        private RectangleF _bounds;
        private static RenderTarget2D _partyTeleportTexture;
        private Vector2 _mousepos;

        /// <summary>
        /// Blocks in-game mouse input
        /// </summary>
        public override bool IsModal => true;

        public TeleportationInterface(NabunassarGame game) : base(game)
        {
            Game.DisableMouseSystems();
            Game.DisableGlowFocus();
        }

        public override void LoadContent()
        {
            _partyTeleportTexture ??= new RenderTarget2D(Game.GraphicsDevice, EntityFactory.HeroRenderWidth * 4, EntityFactory.HeroRenderHeight);
            _partyRectangleSize = new Point(EntityFactory.HeroRenderWidth * 4, EntityFactory.HeroRenderHeight);
            _waves = new WavesShaderEffect(Game, new Size(_partyTeleportTexture.Width,_partyTeleportTexture.Height),.08f);
            _assemblyTP = new TeleportationShaderEffect(Game, true);
            _disassemblyTP = new TeleportationShaderEffect(Game);
        }

        protected override Widget CreateWidget()
        {
            Game.IsMouseVisible = false;
            return new Panel();
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            base.OnAfterAddedWidget(widget);
        }

        public override void Update(GameTime gameTime)
        {
            if (_isTeleporting)
            {
                _disassemblyTP.Update(gameTime);
                _assemblyTP.Update(gameTime);

                return;
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
            {
                this.Close();
            }

            var mouse = MouseExtended.GetState();
            _mousepos = mouse.Position.ToVector2();

            var pos = mouse.Position.ToVector2();

            var imageLength = EntityFactory.HeroRenderWidth * 4;

            var imageWorldPosition = Game.Camera.ScreenToWorld(new Vector2(pos.X - imageLength , pos.Y - EntityFactory.PartyRenderYOffset));
            _partyRenderPosition = new Vector2(imageWorldPosition.X - EntityFactory.PartyRenderXOffset, imageWorldPosition.Y - EntityFactory.PartyRenderYOffset);

            _waves.Update(gameTime);

            var mouseWorldPosition = Game.Camera.ScreenToWorld(pos);
            var boundPos = new Vector2(mouseWorldPosition.X - Game.GameState.Party.Bounds.Size.Width / 2, mouseWorldPosition.Y - Game.GameState.Party.Bounds.Size.Height / 2);
            _bounds = new RectangleF(boundPos, Game.GameState.Party.Bounds.Size);

            var objectLayer = Game.CollisionComponent.Layers[CollisionLayers.Objects];
            var obj = objectLayer.Space.Query(_bounds).FirstOrDefault();
            if (obj != default)
            {
                _waves.SetColor(Color.IndianRed);
            }
            else
            {
                _waves.SetColor();
            }

            if (mouse.WasButtonPressed(MouseButton.Left) && obj == default)
            {
                _isTeleporting = true;
                _disassemblyTP.OnEnd += () =>
                {
                    this.Close();
                    Game.GameState.Party.Visible = true;
                    Game.GameState.Party.SetPosition(new Vector2(_bounds.Position.X - EntityFactory.PartyBoundRenderOffsetX, _bounds.Position.Y));
                };
                Game.GameState.Party.Visible = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (_isTeleporting)
            {

                var tpSb = Game.BeginDraw(effect: _disassemblyTP.Effect);
                tpSb.Draw(_partyTeleportTexture, new Rectangle(_partyRenderPosition.ToPoint(), _partyRectangleSize), new Rectangle(Point.Zero, _partyRectangleSize), Color.White);
                tpSb.End();

                var partyPosVector = Game.GameState.Party.GetPosition();
                var partyPos = new Point(((int)partyPosVector.X) - EntityFactory.PartyRenderXOffset, ((int)partyPosVector.Y) - EntityFactory.PartyRenderYOffset);

                tpSb = Game.BeginDraw(effect: _assemblyTP.Effect);
                tpSb.Draw(_partyTeleportTexture, new Rectangle(partyPos, _partyRectangleSize), new Rectangle(Point.Zero, _partyRectangleSize), Color.White);
                tpSb.End();

                return;
            }

            SpriteBatchKnowed sb;
            if(!_isCaptured)
            {
                CaptureTexture();
            }

            sb = Game.BeginDraw(effect: _waves.Effect);
            sb.Draw(_partyTeleportTexture, new Rectangle(_partyRenderPosition.ToPoint(), _partyRectangleSize),  new Rectangle(Point.Zero, _partyRectangleSize), Color.White);
            sb.End();

            if (Game.IsDrawBounds)
            {
                sb = Game.BeginDraw();
                sb.DrawRectangle(_bounds, Color.Red);
                sb.End();
            }
        }

        private void CaptureTexture()
        {
            SpriteBatchKnowed sb;
            var partyHeroes = Game.GameState.Party.AsEnumerable();
            if (Game.GameState.Party.ViewDirection == Struct.Direction.Right)
            {
                partyHeroes = partyHeroes.Reverse();
            }

            Game.SetRenderTarget(_partyTeleportTexture);
            Game.ClearRenderTarget(Color.Black);
            Game.ClearRenderTarget(Color.Transparent);

            sb = Game.BeginDraw(false);

            var xOffset = 0;

            foreach (var hero in partyHeroes)
            {
                sb.Draw(hero.Sprite, new Vector2(xOffset, 0));
                xOffset += EntityFactory.HeroRenderWidthOffset;
            }

            sb.End();

            Game.SetRenderTarget(null);

            _isCaptured = true;
        }

        public override void Dispose()
        {
            Game.EnableSystems();
            _waves.Dispose();
            Game.IsMouseVisible = true;
            base.Dispose();
        }
    }
}
