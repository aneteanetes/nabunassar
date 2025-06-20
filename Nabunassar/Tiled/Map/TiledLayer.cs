﻿using System.Diagnostics;

namespace Nabunassar.Tiled.Map
{
    [DebuggerDisplay("{name}")]
    public class TiledLayer
    {
        public string name { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public List<TiledPolygon> Tiles { get; set; } = new List<TiledPolygon>();

        public List<List<TiledPolygon>> TilesArray { get; set; } = new();
    }
}