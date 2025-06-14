using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class CollisionsComponent : ICollisionActor
    {
        public IShapeF Bounds { get; private set; }

        public Vector2 PrevBoundPosition { get; set; }

        public Entity Entity { get; private set; }

        public ObjectType ObjectType { get; private set; }

        public Color[] BoundsData { get; private set; }

        public int TextureWidth { get; private set; }

        private Dictionary<ulong, Texture2D> textureCache = [];

        private CollisionEventHandler _onCollistion;

        public string LayerName { get; private set; } = null;

        public CollisionsComponent(NabunassarGame game, RectangleF bounds, ObjectType objType, Entity entity,string layer=null, CollisionEventHandler onCollistion=null)
        {
            LayerName = layer;
            Entity = entity;
            ObjectType = objType;
            Bounds = bounds;
            _onCollistion = onCollistion;

            var key = (ulong)bounds.Width*(ulong)bounds.Height;
            if (!textureCache.ContainsKey(key))
            {
                var width = (int)bounds.Width;
                var height = (int)bounds.Height;
                var texture = new Texture2D(game.GraphicsDevice, width, height);
                var data = new Color[key];
                Array.Fill(data,Color.White);
                texture.SetData(data);
                BoundsData = data;
                TextureWidth = texture.Width;
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (_onCollistion == null)
                return;

            var otherCollision = collisionInfo.Other.As<CollisionsComponent>();

            var thisDesc = Entity.Get<DescriptorComponent>();
            var otherDesc = otherCollision.Entity.Get<DescriptorComponent>();

            _onCollistion.Invoke(collisionInfo,this.Entity,otherCollision.Entity);
        }
    }

    public delegate void CollisionEventHandler(CollisionEventArgs collisionInfo, Entity host, Entity another);
}