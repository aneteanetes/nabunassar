using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class MouseControlSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        Dictionary<string, Dictionary<int, MouseCursor>> CursorMap = new();

        public MouseControlSystem(NabunassarGame game) : base(Aspect.All(typeof(CursorComponent)))
        {
            _game = game;
        }

        private bool isInitialized = false;

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        private void Initialize()
        {
            var cursor = _game.GameState.Cursor;

            var map = new Dictionary<int, MouseCursor>();

            CursorMap.Add("busy", map);

            foreach (var anim in cursor.Animations)
            {
                foreach (var frame in anim.Frames)
                {
                    var idx = frame.FrameIndex;
                    var region = cursor.SpriteSheet.TextureAtlas[idx];

                    var texture = new Texture2D(_game.GraphicsDevice, 16, 16);
                    Color[] data = new Color[16 * 16];
                    cursor.SpriteSheet.TextureAtlas.Texture.GetData(0, new Rectangle(region.X, region.Y, region.Width, region.Height), data, 0, 16 * 16);
                    texture.SetData(data);

                    var mouseCursor = MouseCursor.FromTexture2D(texture, 0, 0);

                    map[idx] = mouseCursor;
                }
            }

            isInitialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_game.GameState?.Cursor == default)
                return;

            if (!isInitialized)
                Initialize();

            var animatedSprite = _game.GameState.Cursor.AnimatedSprite;
            animatedSprite.Update(gameTime);

            var mouse = MouseExtended.GetState();

            var party = _game.GameState.Party;
            if (party == null)
                return;

            var partyMoving = party.Entity.Get<MoveComponent>();
            if (partyMoving.IsMoving())
            {
                if (animatedSprite.CurrentAnimation != "busy")
                {
                    animatedSprite.SetAnimation("busy")
                        .OnAnimationEvent += Ctrl_OnAnimationEvent;
                }
            }
            else if (animatedSprite.CurrentAnimation != "cursor")
            {
                _game.GameState.Cursor.SetCursor("cursor");
            }
        }

        private void Ctrl_OnAnimationEvent(IAnimationController ctrl, AnimationEventTrigger arg2)
        {
            Mouse.SetCursor(CursorMap["busy"][ctrl.CurrentFrame]);
        }
    }
}