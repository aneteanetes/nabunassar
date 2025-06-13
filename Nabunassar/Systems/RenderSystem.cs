using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.ECS
{
    internal class RenderSystem : EntityDrawSystem, IUpdateSystem
    {
        private ComponentMapper<CollisionsComponent> _collistionsMapper;
        private ComponentMapper<RenderComponent> _renderMapper;
        private bool isDrawBounds = false;
        private NabunassarGame _game;

        public RenderSystem(NabunassarGame game) : base(Aspect.One(typeof(RenderComponent),typeof(CollisionsComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _renderMapper = mapperService.GetMapper<RenderComponent>();
            _collistionsMapper = mapperService.GetMapper<CollisionsComponent>();
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.B))
                isDrawBounds = !isDrawBounds;

            foreach (var entityId in ActiveEntities)
            {
                var render = _renderMapper.Get(entityId);
                if (render.Sprite is AnimatedSprite animatedSprite)
                    animatedSprite.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = _game.BeginDraw();
            foreach (var entityId in ActiveEntities)
            {
                if (this.GetEntity(entityId).Get<PlayerComponent>() != null)
                    Console.WriteLine();

                var render = _renderMapper.Get(entityId);
                sb.Draw(render.Sprite, render.Position, render.Rotation, Vector2.One);

                if (isDrawBounds)
                {
                    var collisions = _collistionsMapper.Get(entityId);
                    if (collisions != null)
                        sb.DrawRectangle(collisions.Bounds.As<RectangleF>(), Color.Red);
                }
            }
        }
    }
}
