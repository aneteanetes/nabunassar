using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Nabunassar.Components;
using Nabunassar.Components.Effects;
using Nabunassar.Entities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Monogame.Extended;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces;

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
        private FontSystem _retron;
        private Vector2 _partyRenderPosition;
        private RectangleF _bounds;
        private static RenderTarget2D _partyTeleportTexture;
        private Vector2 _mousepos;
        private TeleportationAbility _teleportationAbility;
        private RectangleF _availableBounds;
        private Label _warningText;
        private Vector2 _boundPos;
        private RectangleF _queryRec;
        private Ray2 _ray;
        private float _blockedDistance;
        private Vector2 _boundPosition;
        private Vector2 _boundPositionLeft;
        private Vector2 _boundPositionRight;

        /// <summary>
        /// Blocks in-game mouse input
        /// </summary>
        public override bool IsModal => true;

        public TeleportationInterface(NabunassarGame game, TeleportationAbility teleportationAbility, int availableDistanceInGameMeters) : base(game)
        {
            _teleportationAbility = teleportationAbility;
            var _availableSize = availableDistanceInGameMeters * Game.GameState.GameMeterMeasure;
            _availableBounds = Game.GameState.Party.Bounds.Add(_availableSize);
            Game.DisableMouseSystems();
            Game.DisableGlowFocus();
        }

        public override void LoadContent()
        {
            _partyTeleportTexture ??= new RenderTarget2D(Game.GraphicsDevice, MapEntityFactory.HeroRenderWidth * 4, MapEntityFactory.HeroRenderHeight);
            _partyRectangleSize = new Point(MapEntityFactory.HeroRenderWidth * 4, MapEntityFactory.HeroRenderHeight);
            _waves = new WavesShaderEffect(Game, new Size(_partyTeleportTexture.Width,_partyTeleportTexture.Height),.08f);
            _assemblyTP = new TeleportationShaderEffect(Game, true);
            _disassemblyTP = new TeleportationShaderEffect(Game);
            _retron = Content.LoadFont(Fonts.Retron);
        }

        protected override Widget CreateWidget()
        {
            Game.IsMouseVisible = false;
            var panel = new Panel();


            var tooltipStyle = Stylesheet.Current.TooltipStyle;
            _warningText = new Label()
            {
                Background = tooltipStyle.Background,
                Font = _retron.GetFont(28),
                Padding = tooltipStyle.Padding,
                Visible = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Top = 75
            };

            panel.Widgets.Add(_warningText);

            return panel;
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

            var mouse = MouseExtended.GetState();

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Escape) || mouse.WasButtonPressed(MouseButton.Right))
            {
                this.Close();
            }

            _mousepos = mouse.Position.ToVector2();
            BoundMousePos();

            var pos = mouse.Position.ToVector2();

            var imageLength = MapEntityFactory.HeroRenderWidth * 4;

            var imageWorldPosition = Game.Camera.ScreenToWorld(new Vector2(pos.X - imageLength , pos.Y - MapEntityFactory.PartyRenderYOffset));
            _partyRenderPosition = new Vector2(imageWorldPosition.X - MapEntityFactory.PartyRenderXOffset, imageWorldPosition.Y - MapEntityFactory.PartyRenderYOffset);

            _waves.Update(gameTime);

            var mouseWorldPosition = Game.Camera.ScreenToWorld(pos);
#warning rank 5: item TP TODO
            var boundPos = new Vector2(mouseWorldPosition.X - Game.GameState.Party.Bounds.Size.Width / 2, mouseWorldPosition.Y - Game.GameState.Party.Bounds.Size.Height / 2);
            _bounds = new RectangleF(boundPos, Game.GameState.Party.Bounds.Size);


            bool isCollided = false;

            var objectLayer = Game.CollisionComponent.Layers[CollisionLayers.Objects];
            var obj = objectLayer.Space.Query(_bounds).FirstOrDefault();
            if (obj != default)
            {
                _waves.SetColor(Color.IndianRed);
                isCollided = true;
            }
            else
            {
                _waves.SetColor();
            }

            if (!isCollided) 
            {
                if (CheckObstacleLength())
                {
                    _warningText.Visible = true;
                    _warningText.Text = $"{Game.Strings["GameTexts"]["Maximum length of obstacle"]} ({(_blockedDistance / Game.GameState.GameMeterMeasure).ToString("0.0")}{Game.Strings["UI"]["m"]}) {Game.Strings["GameTexts"]["obstacle length should not exceed"]}: {(_teleportationAbility.AvailableDistance / Game.GameState.GameMeterMeasure)}{Game.Strings["UI"]["m"]} [{_teleportationAbility.AvailableDistanceFormula()}]";

                    _waves.SetColor(Color.IndianRed);
                    return;
                }
            }

            if (mouse.WasButtonPressed(MouseButton.Left) && !isCollided)
            {
                _teleportationAbility.DoTeleport();
                _isTeleporting = true;
                _disassemblyTP.OnEnd += () =>
                {
                    this.Close();
                    Game.GameState.Party.Visible = true;
                    Game.GameState.Party.SetPosition(new Vector2(_bounds.Position.X - MapEntityFactory.PartyBoundRenderOffsetX, _bounds.Position.Y));
                };
                Game.GameState.Party.Visible = false;
            }            
        }

        private bool CheckObstacleLength()
        {
            var positionTo = _bounds.Position;
            var positionFrom = Game.GameState.Party.Bounds.Position;

            var halfWidth = _bounds.Width / 2;
            var halfHeight = _bounds.Height / 2;

            positionFrom = new Vector2(positionFrom.X + halfWidth, positionFrom.Y + halfHeight);
            positionTo = new Vector2(positionTo.X + halfWidth, positionTo.Y + halfHeight);

            _ray = new Ray2(positionFrom, positionTo);
            var partyBounds = Game.GameState.Party.Bounds;

            var position = new Vector2(Math.Min(_bounds.X, partyBounds.X) + halfWidth, Math.Min(_bounds.Y, partyBounds.Y) + halfHeight);
            var size = new SizeF(Math.Max(_bounds.X, partyBounds.X) - Math.Min(_bounds.X, partyBounds.X), Math.Max(_bounds.Y, partyBounds.Y) - Math.Min(_bounds.Y, partyBounds.Y));

            _queryRec = new RectangleF(position,size);

            var objectLayer = Game.CollisionComponent.Layers[CollisionLayers.Objects];
            var collideds = objectLayer.Space.Query(_queryRec);

            _blockedDistance = 0f;
            _boundPosition = Vector2.Zero;

            var pointSize = new SizeF(1, 1);
            //var normalizedRay = _ray.NormalizeDirection();
            foreach (var collideGroup in collideds.GroupBy(x => x.As<MapObject>().Parent))
            {
#warning distance calc for complex bound objects calculates by first
                var collided = collideGroup.FirstOrDefault();


                var collidedBound = collided.Bounds.BoundingRectangle;

                var origin = collidedBound.GetOrigin();

                _boundPos = ShortenRay(_ray.Position, _ray.Direction, origin);

                var currentLeft = _boundPositionLeft = ShortenRay(_ray.Position, _ray.Direction, collidedBound.Position);
                var currentRight = _boundPositionRight = ShortenRay(_ray.Position, _ray.Direction, new Vector2(collidedBound.Right, collidedBound.Bottom));

                var boundPoint = new RectangleF(_boundPos, pointSize);

                var colliding = collidedBound.Intersection(boundPoint);
                if (!colliding.IsEmpty)
                {
                    _blockedDistance += Vector2.Distance(currentLeft, currentRight);
                }
            }

            return _blockedDistance > _teleportationAbility.AvailableDistance;
        }

        public static double CalculateDistance2D(double x1, double y1, double x2, double y2)
        {
            double deltaX = x2 - x1;
            double deltaY = y2 - y1;
            return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }

        /// <summary>
        /// degenerated method
        /// </summary>
        /// <param name="ray1Start"></param>
        /// <param name="ray1End"></param>
        /// <param name="ray2End"></param>
        /// <returns></returns>
        public Vector2 ShortenRay(Vector2 ray1Start, Vector2 ray1End, Vector2 ray2End)
        {
            // Вычисляем длину луча ray2
            float targetLength = Vector2.Distance(ray1Start, ray2End);

            // Вычисляем направление луча ray1 и нормализуем его
            Vector2 direction = ray1End - ray1Start;
            if (direction != Vector2.Zero)
                direction.Normalize();

            // Новая конечная точка ray1
            return ray1Start + direction * targetLength;
        }

        private void BoundMousePos()
        {
            var pos = Game.Camera.ScreenToWorld(_mousepos);

            if (!_availableBounds.Substract(5).InBounds(pos, out var boundedPos))
            {
                _warningText.Visible = true;
                _warningText.Text = $"{Game.Strings["GameTexts"]["Max radius reached"]}: {_teleportationAbility.LastRoll.Result.ToValue()} [{_teleportationAbility.LastRoll.Result.ToFormula()}]";
            }
            else
            {
                _warningText.Visible = false;
            }

            if (!_availableBounds.InBounds(pos, out var newPos))
            {
                var newScreenPosition = Game.Camera.WorldToScreen(newPos);
                _mousepos = newScreenPosition;
                var holdPos = newScreenPosition.ToPoint();
                Mouse.SetPosition(holdPos.X, holdPos.Y);
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
                var partyPos = new Point(((int)partyPosVector.X) - MapEntityFactory.PartyRenderXOffset, ((int)partyPosVector.Y) - MapEntityFactory.PartyRenderYOffset);

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
                sb.DrawRectangle(_availableBounds, Color.Purple);
                sb.DrawPoint(_boundPos, Color.Yellow, 3);
                sb.DrawRectangle(_queryRec,Color.RoyalBlue);
                sb.DrawLine(_ray.Position,_ray.Direction,Color.LightYellow);
                sb.DrawPoint(_ray.Direction, Color.OrangeRed, 2);


                sb.DrawPoint(_boundPosition, Color.MediumPurple, 4);

                sb.DrawPoint(_boundPositionLeft, Color.GreenYellow, 4);
                sb.DrawPoint(_boundPositionRight, Color.DarkOliveGreen, 4);

                sb.End();
                sb = Game.BeginDraw(false);
                sb.DrawString(_retron.GetFont(40), _blockedDistance.ToString(), new Vector2(150,150), Color.White);
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
                xOffset += MapEntityFactory.HeroRenderWidthOffset;
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
