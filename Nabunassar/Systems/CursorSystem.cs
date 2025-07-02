using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class CursorSystem : BaseSystem
    {
        ComponentMapper<MapObject> _gameObjectComponentMapper;

        public CursorSystem(NabunassarGame game) : base(game,Aspect.All(typeof(CursorComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _gameObjectComponentMapper = mapperService.GetMapper<MapObject>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            var mouse = MouseExtended.GetState();

            var _worldPosition = Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

            foreach (var entityId in ActiveEntities)
            {
                var gameObject = _gameObjectComponentMapper.Get(entityId);

                gameObject.Position = new Vector2(_worldPosition.X, _worldPosition.Y);
            }
        }
    }
}