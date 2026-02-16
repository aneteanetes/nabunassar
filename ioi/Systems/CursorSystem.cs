using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using ioi.Components;

namespace ioi.Systems
{
    internal class CursorSystem : BaseSystem
    {
        ComponentMapper<MapObject> _gameObjectComponentMapper;

        public CursorSystem(GameHost game) : base(game,Aspect.All(typeof(CursorComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _gameObjectComponentMapper = mapperService.GetMapper<MapObject>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            var mouse = MouseExtended.GetState();

            var _worldPosition = Game.CameraMain.ScreenToWorld(mouse.X, mouse.Y);

            foreach (var entityId in ActiveEntities)
            {
                var gameObject = _gameObjectComponentMapper.Get(entityId);

                gameObject.Position = new Vector2(_worldPosition.X, _worldPosition.Y);
            }
        }
    }
}