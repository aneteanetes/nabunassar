using Geranium.Reflection;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Entities.Game;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;
using static Assimp.Metadata;

namespace Nabunassar.Systems
{
    internal class MouseMapObjectFocusSystem : BaseSystem
    {
        private ComponentMapper<FocusWidgetComponent> focusWidgetMapper;

        public MouseMapObjectFocusSystem(NabunassarGame game) : base(game, Aspect.One(typeof(FocusWidgetComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            focusWidgetMapper = mapperService.GetMapper<FocusWidgetComponent>();

        }

        private FocusWidgetComponent _focusedWidgetComponent;

        public override void Update(GameTime gameTime, bool isSystem)
        {
            ICollisionActor actor = null;
            GameObject focusedObject = null;
            MapObject focusedMapObject = null;

            FocusWidgetComponent focusWidgetComponent = null;

            var mouse = MouseExtended.GetState();
            var mousePos = Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

            var bounds = new RectangleF(mousePos, new SizeF(2, 2));

            foreach (var layerItem in Game.CollisionComponent.Layers)
            {
                if (layerItem.Key == "cursor")
                    continue;

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

            if (focusWidgetComponent == null)
            {
                _focusedWidgetComponent = null;
                Game.RemoveDesktopWidgets<TitleWidget>();
                return;
            }

            if (focusWidgetComponent == _focusedWidgetComponent)
                return;

            Game.RemoveDesktopWidgets<TitleWidget>();

            _focusedWidgetComponent = focusWidgetComponent;


            Vector2 position = mouse.Position.ToVector2();

            if (focusWidgetComponent!=default)
            {
                if(focusedMapObject!=null)
                {
                    var tiledObject = focusedMapObject.Entity.Get<TiledObject>();
                    if (tiledObject != default)
                    {
                        position = new Vector2(((float)tiledObject.x) + tiledObject.width / 2, ((float)tiledObject.y));
                        position = Game.Camera.WorldToScreen(position-new Vector2(16));
                    }
                }

                var screenWidget = focusWidgetComponent.WidgetFactory?.Invoke(new FocusEvent()
                {
                    IsFocused = true,
                    Object = focusWidgetComponent.GameObject,
                    Position = position
                });
                Game.AddDesktopWidget(screenWidget);

            }
        }
    }
}
