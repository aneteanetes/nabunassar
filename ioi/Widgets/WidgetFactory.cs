using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using ioi.Entities.Data.Items;
using ioi.Entities.Game;
using ioi.Monogame.Content;
using ioi.Widgets.Base;
using ioi.Widgets.UserInterfaces;
using ioi.Widgets.UserInterfaces.ContextMenus.Radial;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using ioi.Widgets.Views;

namespace ioi.Widgets
{
    internal class WidgetFactory
    {
        private Texture2D _windowBackground;
        private FontSystem _font;

        GameHost Game { get; }

        GameContentManager Content => Game.Content;

        public WidgetFactory(GameHost game)
        {
            Game = game;
        }

        public void LoadContent()
        {
            _windowBackground = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
            _font = Content.LoadFont(Fonts.BitterSemiBold);
        }

        public DialogueMenu OpenDialogue(GameObject gameObject)
        {
            Game.GameState.Cursor.SetCursor("cursor");

            var dialogue = new DialogueMenu(Game, gameObject);
            return Game.AddDesktopWidget(dialogue);
        }
    }
}
