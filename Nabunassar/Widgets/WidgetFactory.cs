using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Content;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets
{
    internal class WidgetFactory
    {
        private Texture2D _windowBackground;
        private FontSystem _font;

        NabunassarGame Game { get; }

        NabunassarContentManager Content => Game.Content;

        public WidgetFactory(NabunassarGame game)
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

        public ItemPanel ItemsContainer(List<ItemView> items, Action<Panel, Item> click, Action<Panel, Item> dblClick)
        {
            return new ItemPanel(items, _font, click, dblClick);
        }
    }
}
