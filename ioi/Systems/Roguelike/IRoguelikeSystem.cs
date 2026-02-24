namespace ioi.Systems.Roguelike
{
    internal interface IRoguelikeSystem : IDisposable
    {
        void Draw(GameTime gameTime);
        void LoadContent();

        void Update(GameTime gameTime);
    }
}
