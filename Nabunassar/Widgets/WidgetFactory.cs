﻿using Nabunassar.Entities.Game;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial;

namespace Nabunassar.Widgets
{
    internal class WidgetFactory
    {
        NabunassarGame Game { get; }

        public WidgetFactory(NabunassarGame game)
        {
            Game = game;
        }

        public DialogueMenu OpenDialogue(GameObject gameObject)
        {
            Game.GameState.Cursor.SetCursor("cursor");

            var dialogue = new DialogueMenu(Game, gameObject);
            return Game.AddDesktopWidget(dialogue);
        }

        public RadialMenu OpenRadial(GameObject gameObject, Vector2 position)
        {
            Game.GameState.Cursor.SetCursor("cursor");
            return Game.AddDesktopWidget(new RadialMenu(Game, gameObject, position));
        }
    }
}
