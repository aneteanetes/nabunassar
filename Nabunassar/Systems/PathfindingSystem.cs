using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Components.Inactive;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;

namespace Nabunassar.Systems
{
    internal class PathfindingSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<PlayerComponent> _playerManager;
        ComponentMapper<PathfindingComponent> _pathingManager;
        ComponentMapper<RenderComponent> _renderManager;
        ComponentMapper<BoundsComponent> _collideManager;
        IEnumerator<Vector2> enumerator;


        public PathfindingSystem(NabunassarGame game):base(Aspect.One(typeof(PlayerComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerManager = mapperService.GetMapper<PlayerComponent>();
            _renderManager = mapperService.GetMapper<RenderComponent>();
            _collideManager = mapperService.GetMapper<BoundsComponent>();
            _pathingManager = mapperService.GetMapper<PathfindingComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                ProcessGoRoguePath(entity);
            }
        }

        private void ProcessGoRoguePath(int entity)
        {
            var playerRender = _renderManager.Get(entity);
            var playerBounds = _collideManager.Get(entity);
            var pathing = _pathingManager.Get(entity);

            if (pathing.Path != default)
            {
                if (enumerator == null)
                    enumerator = pathing.Path.GetEnumerator();

                if (enumerator.MoveNext())
                {
                    var pos = _game.Camera.WorldToScreen(enumerator.Current.X, enumerator.Current.Y);
                    playerRender.Position = pos;
                    playerBounds.Position = pos;
                }
                else
                {
                    enumerator = null;
                    pathing.Path = default;
                }
            }
        }

    }
}
