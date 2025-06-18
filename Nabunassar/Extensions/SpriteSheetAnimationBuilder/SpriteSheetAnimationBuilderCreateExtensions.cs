using Geranium.Reflection;
using MonoGame.Extended.Graphics;
using System.Linq.Expressions;

namespace Nabunassar
{
    internal static class SpriteSheetAnimationBuilderCreateExtensions
    {
        private static Func<string, SpriteSheet,object> New;

        public static SpriteSheetAnimationBuilder CreateBuilder(this SpriteSheet spriteSheet)
        {
            if (New == default)
                CreateLambda();

            return New(Guid.NewGuid().ToString(),spriteSheet).As<SpriteSheetAnimationBuilder>();
        }

        private static void CreateLambda()
        {
            var constructor = typeof(SpriteSheetAnimationBuilder).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)[0];

            ParameterExpression[] @params = [
                Expression.Parameter(typeof(string)),
                Expression.Parameter(typeof(SpriteSheet))
            ];
            Expression[] arguments = @params;

            New = Expression.Lambda<Func<string, SpriteSheet, object>>(Expression.New(constructor, arguments), @params).Compile();
        }
    }
}
