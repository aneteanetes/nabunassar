using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.ECS;
using Nabunassar.Systems;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void InitGameWorld()
        {
            Game.InitializeCollisions();

            MapWorld = new WorldBuilderProxy(this)
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

            MapEntityFactory = new Entities.MapEntityFactory(this);
        }

        public void InitCombatWorld()
        {
            CombatWorld = new WorldBuilderProxy(this)
                .Build();
        }

        public void DisposeGameWorld()
        {
            Game.MapWorld.IsEnabled = false;
            Game.MapWorld.Dispose();
            Game.DisposeCollisionComponent();
        }

        public HashSet<Type> DisabledWorldSystems = new();
        public HashSet<Type> RegisteredSystems = new();

        public void DestoryEntity(Entity entity)
        {
            var collision = entity.Get<MapObject>();
            this.MapWorld.DestroyEntity(entity);
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
            private readonly NabunassarGame _game;

            public WorldBuilderProxy(NabunassarGame game, WorldBuilder worldBuilder=default)
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
