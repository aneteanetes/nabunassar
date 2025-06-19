using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class MoveSystem : EntityUpdateSystem, IDrawSystem
    {
        NabunassarGame _game;
        ComponentMapper<MoveComponent> _moveComponentMapper;
        ComponentMapper<PositionComponent> _positionComponentMapper;
        ComponentMapper<BoundsComponent> _boundComponentMapper;

        public MoveSystem(NabunassarGame game) : base(Aspect.One(typeof(MoveComponent), typeof(PositionComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _moveComponentMapper = mapperService.GetMapper<MoveComponent>();
            _positionComponentMapper = mapperService.GetMapper<PositionComponent>();
            _boundComponentMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var move = _moveComponentMapper.Get(entityId);
                var position = _positionComponentMapper.Get(entityId) ?? _boundComponentMapper.Get(entityId);

                if (move.IsMoving())
                {
                    float xOffset = 0;
                    float yOffset = 0;

                    var renderNewPos = Vector2.Zero;
                    var newPos = Vector2.Zero;

                    if (move.Ray2 == default)
                    {
                        switch (move.MoveDirection)
                        {
                            case Direction.Up:
                                yOffset -= move.MoveSpeed;
                                break;
                            case Direction.Down:
                                yOffset += move.MoveSpeed;
                                break;
                            case Direction.Left:
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.Right:
                                xOffset += move.MoveSpeed;
                                break;
                            case Direction.UpLeft:
                                yOffset -= move.MoveSpeed;
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.UpRight:
                                yOffset -= move.MoveSpeed;
                                xOffset += move.MoveSpeed;
                                break;
                            case Direction.DownLeft:
                                yOffset += move.MoveSpeed;
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.DownRight:
                                yOffset += move.MoveSpeed;
                                xOffset += move.MoveSpeed;
                                break;
                            default:
                                break;
                        }

                        newPos = new Vector2(position.Position.X + xOffset, position.Position.Y + yOffset);
                    }
                    else
                    {
                        var boundT = move.MoveSpeed / Vector2.Distance(position.Position, move.Ray2.Direction);
                        newPos = Vector2.Lerp(position.Position, move.Ray2.Direction, boundT);
                    }

                    //reset speed after changing position
                    move.ResetMoveSpeed();

                    // reset moving if position reached //new RectangleF(position.Position,position.BoundsComponent.Bounds.BoundingRectangle.Size)
                    if (position.BoundsComponent.Bounds.Intersects(new RectangleF(move.TargetPosition, new SizeF(1, 1))))
                    {
                        move.Stop();
                    }

                    // setting position
                    position.SetPosition(newPos);

                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var move = _moveComponentMapper.Get(entityId);
                var ray = move.Ray2;
                if(ray!=default && _game.IsDrawBounds)
                {
                    var sb = _game.BeginDraw();
                    sb.DrawLine(ray.Position, ray.Direction, Color.Red, 1);
                }
            }
        }
    }
}