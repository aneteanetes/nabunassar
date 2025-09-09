using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.ECS;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Extended;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Systems
{
    internal class MapObjectFocusSystem : BaseSystem
    {
        private FocusWidgetComponent _focusedWidgetComponent;
        private HashSet<string> _focusingLayers;
        private KeyValuePair<string, Layer>[] _availableLayers;
        private ComponentMapper<FocusWidgetComponent> focusWidgetMapper;

        public MapObjectFocusSystem(NabunassarGame game) : base(game, Aspect.One(typeof(FocusWidgetComponent)))
        {
            _focusingLayers = [CollisionLayers.Revealed, CollisionLayers.Objects];
            _availableLayers = Game.CollisionComponent.Layers.Where(x => _focusingLayers.Contains(x.Key)).ToArray();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            focusWidgetMapper = mapperService.GetMapper<FocusWidgetComponent>();

        }

        public override void Update(GameTime gameTime, bool isSystem)
        {
            if(!Game.IsMouseMoveAvailable)
                return;

            ICollisionActor actor = null;
            GameObject focusedObject = null;
            MapObject focusedMapObject = null;

            FocusWidgetComponent focusWidgetComponent = null;

            var mouse = MouseExtended.GetState();
            var mousePos = Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

            var bounds = new RectangleF(mousePos, new SizeF(2, 2));

            foreach (var layerItem in _availableLayers)
            {
                var layer = layerItem.Value;

                actor = layer.Space.Query(bounds).FirstOrDefault();
                if (actor != null)
                {
                    focusedMapObject = actor.As<MapObject>();

                    if (focusedMapObject != default)
                    {
                        focusedObject = focusedMapObject.GameObject;

                        if (focusedObject != default)
                        {
                            var widget = focusWidgetMapper.Get(focusedMapObject.Entity);
                            if (widget != null && widget.GameObject == focusedObject)
                                focusWidgetComponent = widget;
                        }

                    }
                }
            }

            if (focusWidgetComponent == null && _focusedWidgetComponent!=null)
            {
                RenderSystem.DisableGlow();

                _focusedWidgetComponent = null;
                Game.RemoveDesktopWidgets<TitleWidget>();
                Game.GameState.Cursor.SetCursor("cursor");
                return;
            }

            if (focusWidgetComponent == _focusedWidgetComponent)
                return;

            Game.RemoveDesktopWidgets<TitleWidget>();
            Game.GameState.Cursor.SetCursor("cursor");

            _focusedWidgetComponent = focusWidgetComponent;


            Vector2 position = mouse.Position.ToVector2();

            if (focusWidgetComponent!=default)
            {
                if(focusedMapObject!=null)
                {
                    var tiledBase = focusWidgetComponent.GameObject.Entity.Get<TiledBase>();
                    if (tiledBase != default)
                    {
                        position = tiledBase.Position;
                        position = Game.Camera.WorldToScreen(position);
                    }
                }

                var screenWidget = focusWidgetComponent.WidgetFactory?.Invoke(new FocusEvent()
                {
                    IsFocused = true,
                    Object = focusWidgetComponent.GameObject,
                    Position = position
                });
                Game.AddDesktopWidget(screenWidget);

                var cursor = Game.DataBase.GetFromDictionary("Data/Interface/ObjectTypeCursors.json", focusWidgetComponent.GameObject.ObjectType.ToString());
                if (cursor != default)
                    Game.GameState.Cursor.SetCursor(cursor);

                focusWidgetComponent.OnFocus?.Invoke(focusWidgetComponent.GameObject);
            }
        }
    }
}
