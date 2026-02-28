using Geranium.Reflection;
using MonoGame.Extended.Graphics;
using ioi.Components;
using ioi.Entities.Data.Animations;
using ioi.Entities.Game;

namespace ioi.Entities
{
    internal class BattleEntityFactory(GameHost game) : BaseEntityFactory(game, game.OldECSBattle)
    {
        public void FullfillEncounter(Encounter encounter)
        {
            foreach (var creature in encounter.Creatures)
            {
                if(creature == null) 
                    continue;

                var entity = creature.Entity = CreateEntity($"encounter creature with id {{{creature.CreatureId}}} and in slot {{{encounter.Creatures.GetOctaPosition(creature)}}}");
                string initialAnimation = null;

                var animationsFilePath = creature.GetPropertyValue<string>("AnimationsFile");
                if (animationsFilePath.IsNotEmpty())
                {
                    var animationFile = Game.Content.Load<AnimationFile>(animationsFilePath);

                    var tilesetPath = Path.Combine(Path.GetDirectoryName(animationsFilePath), animationFile.Tileset).Replace("\\", "/");
                    var tileset = Game.Content.LoadTexture(tilesetPath);
                    var atlas = Texture2DAtlas.Create(animationFile.Tileset, tileset, animationFile.TileWidth, animationFile.TileHeight);
                    SpriteSheet spriteSheet = new("SpriteSheet_" + animationFile.Tileset, atlas);

                    foreach (var animation in animationFile.Animations)
                    {
                        if (animationFile.Animations.IndexOf(animation) == 0)
                            initialAnimation = animation.Name;

                        spriteSheet.DefineAnimation(animation.Name, builder =>
                        {
                            builder.IsLooping(animation.IsLoop);
                            foreach (var frame in animation.Frames)
                            {
                                builder.AddFrame(frame.RegionIndex, TimeSpan.FromSeconds(frame.DurationInSeconds));
                            }
                        });
                    }

                    var animatedSprite = new AnimatedSprite(spriteSheet);
                    if (initialAnimation.IsNotEmpty())
                        animatedSprite.SetAnimation(initialAnimation);

                    entity.Attach(animatedSprite);

                    var render = new RenderComponent(Game, animatedSprite, Vector2.Zero, 0);
                    //render.Scale = scale;
                    entity.Attach(render);
                }
            }
        }
    }
}