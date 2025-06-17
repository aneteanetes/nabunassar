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
        private ComponentMapper<BoundsComponent> _collistionsMapper;
        private ComponentMapper<RenderComponent> _renderMapper;
        private bool isDrawBounds = false;
        private NabunassarGame _game;

        public RenderSystem(NabunassarGame game) : base(Aspect.One(typeof(RenderComponent),typeof(BoundsComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _renderMapper = mapperService.GetMapper<RenderComponent>();
            _collistionsMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var render = _renderMapper.Get(entityId);
                if (render!=null && render.Sprite is AnimatedSprite animatedSprite)
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
                if (render != null)
                    sb.Draw(render.Sprite, render.Position, render.Rotation, Vector2.One);

                if (_game.IsDrawBounds)
                {
                    var collisions = _collistionsMapper.Get(entityId);
                    if (collisions != null)
                    {
                        var color = collisions.ObjectType == Struct.ObjectType.Ground ? Color.Blue : Color.Red;
                        sb.DrawRectangle(collisions.Bounds.As<RectangleF>(), color);
                    }
                }
            }

            //sb.End();
        }
    }
}
