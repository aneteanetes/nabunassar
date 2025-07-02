using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Systems;

namespace Nabunassar.ECS
{
    internal class RenderSystem : BaseSystem
    {
        private ComponentMapper<RenderComponent> _renderMapper;
        private ComponentMapper<DescriptorComponent> _descriptorMapper;
        private ComponentMapper<MapObject> _gameObjectMapper;

        public RenderSystem(NabunassarGame game) : base(game, Aspect.One(typeof(RenderComponent),typeof(MapObject)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _renderMapper = mapperService.GetMapper<RenderComponent>();
            _descriptorMapper = mapperService.GetMapper<DescriptorComponent>();
            _gameObjectMapper = mapperService.GetMapper<MapObject>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            var keyboardState = KeyboardExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var render = _renderMapper.Get(entityId);
                if (render!=null && render.Sprite is AnimatedSprite animatedSprite)
                    animatedSprite.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, bool sys)
        {
            var sb = Game.BeginDraw();

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
                    if (render.OnAfterDraw != default)
                        render.OnAfterDraw?.Invoke();
                }
            }

            foreach (var entity in entities)
            {
                if (Game.IsDrawBounds)
                {
                    var mapObj = _gameObjectMapper.Get(entity);
                    if (mapObj != null && mapObj.Bounds != default)
                    {
                        var color = mapObj.BoundsColor;

                        if (color == default)
                        {
                            color = mapObj.ObjectType == Struct.ObjectType.Ground ? Color.Blue : Color.Red;
                        }
                        else
                        {
                            //debug
                        }

                        sb.DrawRectangle(mapObj.Bounds.As<RectangleF>(), color);

                        if (entity.Get<DescriptorComponent>().Name == "party")
                        {
                            sb.DrawRectangle(mapObj.Bounds.BoundingRectangle.Multiple(2), Color.Purple);
                        }
                    }
                }
            }
        }
    }
}
