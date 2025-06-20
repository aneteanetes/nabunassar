using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using static Assimp.Metadata;

namespace Nabunassar.ECS
{
    internal class RenderSystem : EntityDrawSystem, IUpdateSystem
    {
        private ComponentMapper<RenderComponent> _renderMapper;
        private ComponentMapper<DescriptorComponent> _descriptorMapper;
        private ComponentMapper<GameObject> _gameObjectMapper;
        private NabunassarGame _game;

        public RenderSystem(NabunassarGame game) : base(Aspect.One(typeof(RenderComponent),typeof(GameObject)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _renderMapper = mapperService.GetMapper<RenderComponent>();
            _descriptorMapper = mapperService.GetMapper<DescriptorComponent>();
            _gameObjectMapper = mapperService.GetMapper<GameObject>();
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

        private int i;

        public override void Draw(GameTime gameTime)
        {
            var sb = _game.BeginDraw();

            var entities = ActiveEntities.Select(GetEntity).OrderBy(x => x.Get<OrderComponent>().Order).ToList();

            foreach (var entity in entities)
            {
                var descriptor = _descriptorMapper.Get(entity);

                if (descriptor.Name?.Contains("hero") ?? false)
                { }

                var render = _renderMapper.Get(entity);
                if (render != null && render.Sprite.IsVisible)
                {
                    sb.Draw(render.Sprite, render.Position, render.Rotation, render.Scale);
                }
            }

            foreach (var entity in entities)
            {
                if (_game.IsDrawBounds)
                {
                    var gameObj = _gameObjectMapper.Get(entity);
                    if (gameObj != null && gameObj.Bounds != default)
                    {
                        var color = gameObj.BoundsColor;

                        if (color == default)
                        {
                            color = gameObj.ObjectType == Struct.ObjectType.Ground ? Color.Blue : Color.Red;
                        }
                        else
                        {
                            //debug
                        }

                        sb.DrawRectangle(gameObj.Bounds.As<RectangleF>(), color);
                    }
                }
            }
        }
    }
}
