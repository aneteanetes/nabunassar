using MonoGame.Extended.ECS;
using ioi.Components;
using ioi.ECS;
using ioi.Systems;
using ioi.Widgets.UserInterfaces;

namespace ioi
{
    internal partial class GameHost
    {
        public void InitGameWorld()
        {
            Game.InitializeCollisions();

            WorldMap = new WorldBuilderProxy(this)
                .AddSystem(new MinimapSystem(this))
                .AddSystem(new PlayerControllSystem(this))
                .AddSystem(new CursorSystem(this))
                .AddSystem(new RenderSystem(this))
                .AddSystem(new FlickeringSystem(this))
                .AddSystem(new MoveSystem(this))
                .AddSystem(new MouseControlSystem(this))
                .AddSystem(new MapObjectFocusSystem(this))
                .AddSystem(new LightSystem(this))
                .Build();

            EntityFactoryMap = new Entities.MapEntityFactory(this);
        }

        public void InitBattleWorld()
        {
            WorldBattle = new WorldBuilderProxy(this)
                .AddSystem(new SquadRenderSystem(this))
                .Build();

            EntityFactoryBattle = new Entities.BattleEntityFactory(this);
        }

        public void DisposeBattleWorld()
        {
            WorldBattle.IsEnabled = false;
            EntityFactoryBattle = null;
            WorldBattle.Dispose();
        }

        public void DisposeGameWorld()
        {
            Game.WorldMap.IsEnabled = false;
            Game.WorldMap.Dispose();
            Game.DisposeCollisionComponent();
        }

        public HashSet<Type> DisabledWorldSystems = new();
        public HashSet<Type> RegisteredSystems = new();

        public void DestoryEntity(Entity entity)
        {
            var collision = entity.Get<MapObject>();
            this.WorldMap.DestroyEntity(entity);
            CollisionComponent.Remove(collision);
        }

        internal void DisableGlowFocus()
        {
            RenderSystem.DisableGlow();
        }

        public void DisableWorld()
        {
            ChangeGameActive();
        }

        public void EnableWorld()
        {
            ChangeGameActive();
        }

        public void DisableSystems(params Type[] types)
        {
            types.ForEach(x => DisabledWorldSystems.Add(x));
        }

        public void DisableMouseSystems()
        {
            Game.DisableSystems(typeof(PlayerControllSystem), typeof(MapObjectFocusSystem));
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        public void DisableSystemsExcept(params Type[] types)
        {
            var exept = RegisteredSystems.Except(types);
            DisabledWorldSystems = exept.ToHashSet();
        }

        public void EnableSystems()
        {
            DisabledWorldSystems.Clear();
        }

        private class WorldBuilderProxy
        {
            private readonly WorldBuilder _worldBuilder;
            private readonly GameHost _game;

            public WorldBuilderProxy(GameHost game, WorldBuilder worldBuilder=default)
            {
                _game = game;
                _worldBuilder = worldBuilder ?? new WorldBuilder();
            }

            public WorldBuilderProxy AddSystem(BaseSystem system)
            {
                _game.RegisteredSystems.Add(system.GetType());
                _worldBuilder.AddSystem(system);
                return this;
            }

            public World Build() => _worldBuilder.Build();
        }
    }    
}
