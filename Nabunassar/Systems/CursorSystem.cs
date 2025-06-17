using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class CursorSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<CursorComponent> _cursorComponentMapper;
        ComponentMapper<BoundsComponent> _collisionComponentMapper;

        public CursorSystem(NabunassarGame game) : base(Aspect.All(typeof(CursorComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _cursorComponentMapper = mapperService.GetMapper<CursorComponent>();
            _collisionComponentMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = MouseExtended.GetState();


            var _worldPosition = _game.Camera.ScreenToWorld(mouse.X,mouse.Y);

            foreach (var entityId in ActiveEntities)
            {
                var collision = _collisionComponentMapper.Get(entityId);
                
                collision.Bounds.Position =new Vector2(_worldPosition.X, _worldPosition.Y);
            }
        }
    }
}