using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class MoveSystem : EntityUpdateSystem, IDrawSystem
    {
        NabunassarGame _game;
        ComponentMapper<GameObject> _gameObjectComponentMapper;

        public MoveSystem(NabunassarGame game) : base(Aspect.All(typeof(GameObject),typeof(MoveableComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _gameObjectComponentMapper = mapperService.GetMapper<GameObject>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var gameobject = _gameObjectComponentMapper.Get(entityId);

                if (gameobject.IsMoving)
                {
                    float xOffset = 0;
                    float yOffset = 0;

                    var renderNewPos = Vector2.Zero;
                    var newPos = Vector2.Zero;

                    var moveDirection = gameobject.MoveDirection;

                    if (gameobject.Ray2 == default)
                    {
                        switch (moveDirection)
                        {
                            case Direction.Up:
                                yOffset -= gameobject.MoveSpeed;
                                break;
                            case Direction.Down:
                                yOffset += gameobject.MoveSpeed;
                                break;
                            case Direction.Left:
                                xOffset -= gameobject.MoveSpeed;
                                break;
                            case Direction.Right:
                                xOffset += gameobject.MoveSpeed;
                                break;
                            case Direction.UpLeft:
                                yOffset -= gameobject.MoveSpeed;
                                xOffset -= gameobject.MoveSpeed;
                                break;
                            case Direction.UpRight:
                                yOffset -= gameobject.MoveSpeed;
                                xOffset += gameobject.MoveSpeed;
                                break;
                            case Direction.DownLeft:
                                yOffset += gameobject.MoveSpeed;
                                xOffset -= gameobject.MoveSpeed;
                                break;
                            case Direction.DownRight:
                                yOffset += gameobject.MoveSpeed;
                                xOffset += gameobject.MoveSpeed;
                                break;
                            default:
                                break;
                        }

                        newPos = new Vector2(gameobject.Position.X + xOffset, gameobject.Position.Y + yOffset);
                    }
                    else
                    {
                        var boundT = gameobject.MoveSpeed / Vector2.Distance(gameobject.Position, gameobject.Ray2.Direction);
                        newPos = Vector2.Lerp(gameobject.Position, gameobject.Ray2.Direction, boundT);
                    }

                    //reset speed after changing position
                    gameobject.ResetMoveSpeed();

                    // reset moving if position reached //new RectangleF(position.Position,position.BoundsComponent.Bounds.BoundingRectangle.Size)
                    if (gameobject.Bounds.Intersects(new RectangleF(gameobject.TargetPosition, new SizeF(1, 1))))
                    {
                        gameobject.StopMove();
                    }

                    // setting position
                    gameobject.OnMoving?.Invoke(gameobject.Position, newPos);
                    gameobject.SetPosition(newPos);

                    if (gameobject.Position.RoundNew() == gameobject.Ray2.Direction.RoundNew())
                    {
                        gameobject.StopMove();
                    }

                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var gameobject = _gameObjectComponentMapper.Get(entityId);
                var ray = gameobject.Ray2;
                if(ray!=default && _game.IsDrawBounds)
                {
                    var sb = _game.BeginDraw();
                    sb.DrawLine(ray.Position, ray.Direction, Color.Yellow, 1);
                }
            }
        }
    }
}