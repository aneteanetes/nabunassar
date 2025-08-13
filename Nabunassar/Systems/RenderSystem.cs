using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Components.Effects;
using Nabunassar.Entities.Data;
using Nabunassar.Systems;

namespace Nabunassar.ECS
{
    internal class RenderSystem : BaseSystem
    {
        private ComponentMapper<RenderComponent> _renderMapper;
        private ComponentMapper<DescriptorComponent> _descriptorMapper;
        private ComponentMapper<MapObject> _gameObjectMapper;
        private ComponentMapper<EffectComponent> _effectMapper;
        private ComponentMapper<Party> _partyMapper;

        private static bool isGlowNeedDisable = false;

        public static void DisableGlow()
        {
            isGlowNeedDisable = true;
        }

        public RenderSystem(NabunassarGame game) : base(game, Aspect.One(typeof(RenderComponent), typeof(MapObject)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _renderMapper = mapperService.GetMapper<RenderComponent>();
            _descriptorMapper = mapperService.GetMapper<DescriptorComponent>();
            _gameObjectMapper = mapperService.GetMapper<MapObject>();
            _effectMapper = mapperService.GetMapper<EffectComponent>();
            _partyMapper = mapperService.GetMapper<Party>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            var keyboardState = KeyboardExtended.GetState();

            var canUpdateOpacity = this.IsUpdateAvailable(gameTime, 150);

            foreach (var entityId in ActiveEntities)
            {
                var effect = _effectMapper.Get(entityId);
                if (effect != null)
                    effect.Update(gameTime);

                var render = _renderMapper.Get(entityId);
                if (render != null)
                {
                    if (render.OpacityTimer != default && canUpdateOpacity)
                    {
                        render.Opacity += gameTime.ElapsedGameTime.TotalSeconds;

                        var color = render.Sprite.Color;
                        render.Sprite.Color = new Color(color, ((float)render.Opacity)*255);

                        if (render.Opacity >= render.OpacityTimer.TotalSeconds/10)
                        {
                            render.OpacityTimer = default;
                            render.Opacity = 1;
                        }
                    }

                    //updating effect
                    if (render.IsEffect && isGlowNeedDisable)
                        render.Sprite.IsVisible = false;

                    // updating animations
                    if (render.Sprite is AnimatedSprite animatedSprite)
                        animatedSprite.Update(gameTime);
                }
            }

            isGlowNeedDisable = false;
        }

        public override void Draw(GameTime gameTime, bool sys)
        {
            var sb = Game.BeginDraw();

            var entities = ActiveEntities.Select(GetEntity).OrderBy(x => x.Get<OrderComponent>().Order).ToList();

            foreach (var entity in entities)
            {
                var mapObj = _gameObjectMapper.Get(entity);
                if (mapObj != null)
                    if (!mapObj.IsVisible)
                        continue;

                var descriptor = _descriptorMapper.Get(entity);

                if (descriptor.Name?.Contains("hero") ?? false)
                { }

                var render = _renderMapper.Get(entity);
                if (render != null && render.Sprite.IsVisible)
                {
                    if (render.OpacityTimer != default && render.Opacity == 0)
                        continue;

                    var effect = _effectMapper.Get(entity);
                    if (effect != default)
                    {
                        sb.End();
                        sb = Game.BeginDraw(effect: effect.Effect);
                    }

                    sb.Draw(render.Sprite, render.Position, render.Rotation, render.Scale);
                    if (render.OnAfterDraw != default)
                        render.OnAfterDraw?.Invoke();

                    if (effect != default)
                    {
                        sb.End();
                        sb = Game.BeginDraw();
                    }
                }
            }

            if (Game.IsDrawBounds)
            {
                foreach (var entity in entities)
                {
                    var mapObj = _gameObjectMapper.Get(entity);
                    if (mapObj != null && mapObj.Bounds != default)
                    {
                        if (mapObj.Dependant.IsNotEmpty()) // object have complex collision
                            continue;

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
                            var party = _partyMapper.Get(entity);
                            sb.DrawRectangle(party.DistanceMeterRectangle, Color.Purple);
                            sb.DrawRectangle(party.PartyMenuRectangle, Color.LightSkyBlue);
                            sb.DrawRectangle(party.RevealArea, Color.LimeGreen);
                        }
                    }
                }
            }

        }
    }
}
