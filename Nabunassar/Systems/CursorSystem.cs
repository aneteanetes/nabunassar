using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class CursorSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<GameObject> _gameObjectComponentMapper;

        public CursorSystem(NabunassarGame game) : base(Aspect.All(typeof(CursorComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _gameObjectComponentMapper = mapperService.GetMapper<GameObject>();
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = MouseExtended.GetState();

            var _worldPosition = _game.Camera.ScreenToWorld(mouse.X, mouse.Y);

            foreach (var entityId in ActiveEntities)
            {
                var gameObject = _gameObjectComponentMapper.Get(entityId);

                gameObject.Position = new Vector2(_worldPosition.X, _worldPosition.Y);
            }
        }
    }
}