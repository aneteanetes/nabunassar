//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.Content;
//using MonoGame.Extended.Tiled;
//using Nabunassar.Monogame.Content;
//using System.Xml.Linq;

//namespace Nabunassar.Content
//{
//    internal class TiledMapReader
//    {
//        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
//        const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
//        const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

//        public static TiledMap Read(NabunassarContentManager contentManager, Stream asset, string assetName)
//        {
//            XDocument document = XDocument.Load(asset);
//            TiledMap tiledMap = ReadTiledMap(document, assetName);
//            var props = document.Element("properties");
//            ReadTiledMapProperties(props, tiledMap.Properties);

//            ReadTilesets(document, tiledMap, contentManager);

//            ReadLayers(document, tiledMap);
//            return tiledMap;
//        }

//        private static TiledMap ReadTiledMap(XDocument doc, string assetName)
//        {
//            var root = doc.Root;

//            int width = root.GetTagAttrInteger(nameof(width));
//            int height = root.GetTagAttrInteger(nameof(height));
//            int tileWidth = root.GetTagAttrInteger("tilewidth");
//            int tileHeight = root.GetTagAttrInteger("tileheight");
//            Color backgroundColor = root.GetTagAttrColor("backgroundcolor");
//            TiledMapTileDrawOrder renderOrder = ParseOrder(root);
//            TiledMapOrientation orientation = ParseOrientation(root);
//            return new TiledMap(assetName, "deprecated", width, height, tileWidth, tileHeight, renderOrder, orientation, backgroundColor);
//        }

//        private static TiledMapTileDrawOrder ParseOrder(XElement root)
//        {
//            var value = root.GetTagAttrString("renderorder");
//            if (value == "left-up")
//                return TiledMapTileDrawOrder.LeftUp;

//            return TiledMapTileDrawOrder.LeftDown;
//        }

//        private static TiledMapOrientation ParseOrientation(XElement root)
//        {
//            var value = root.GetTagAttrString("orientation");
//            if (value == "orthogonal")
//                return  TiledMapOrientation.Orthogonal;

//            return TiledMapOrientation.Isometric;
//        }

//        private static void ReadTilesets(XDocument document, TiledMap map, NabunassarContentManager contentManager)
//        {
//            foreach (var tileset in document.Root.Elements("tileset"))
//            {
//                int firstGlobalIdentifier = tileset.GetTagAttrInteger("firstgid");
//                TiledMapTileset tiledTileset = ReadTileset(tileset, map, contentManager);
//                map.AddTileset(tiledTileset, firstGlobalIdentifier);
//            }
//        }

//        private static TiledMapTileset ReadTileset(XElement tileset, TiledMap map, NabunassarContentManager contentManager)
//        {
//            var imgsource = Path.GetFileName(tileset.Element("image").GetTagAttrString("source"));
//            var texture = contentManager.Load<Texture2D>($"Assets/Tilesets/" + imgsource);

//            int tileWidth = tileset.GetTagAttrInteger("tilewidth");
//            int tileHeight = tileset.GetTagAttrInteger("tileheight");
//            int tileCount = tileset.GetTagAttrInteger("tilecount");
//            int spacing = 0;
//            int margin = 0;
//            int columns = tileset.GetTagAttrInteger("columns");
            
//            return new TiledMapTileset(texture, "type is unknown", tileWidth, tileHeight, tileCount, spacing, margin, columns);           
//        }

//        private static void ReadLayers(XDocument document, TiledMap map)
//        {
//            foreach (TiledMapLayer item in ReadGroup(document, map))
//            {
//                map.AddLayer(item);
//            }
//        }

//        private static List<TiledMapLayer> ReadGroup(XDocument document, TiledMap map)
//        {
//            List<TiledMapLayer> list = new List<TiledMapLayer>();

//            foreach (var layer in document.Root.Elements("layer"))
//            {
//                list.Add(ReadLayer(layer, map));
//            }

//            return list;
//        }

//        private static TiledMapLayer ReadLayer(XElement layer, TiledMap map)
//        {
//            var name = layer.GetTagAttrString("name");
//            var height = layer.GetTagAttrInteger("height");
//            var width = layer.GetTagAttrInteger("width");

//            var tiledLayer = new TiledMapTileLayer(name,"",width,height,map.TileWidth,map.TileHeight);

//            var dataTag = layer.Element("data");

//            var gids = new List<uint>();

//            var chunks = dataTag.Elements("chunk");
//            if (chunks.Count() > 0)
//            {
//                chunks.ForEach((chunk =>
//                {
//                    var gidsChunk = chunk.Value.Split(",", System.StringSplitOptions.RemoveEmptyEntries).Select(x => uint.Parse(x));
//                    gids.AddRange(gidsChunk);
//                }));
//            }

//            if (gids.Count == 0)
//            {
//                gids = dataTag.Value
//                    .Split(",", System.StringSplitOptions.RemoveEmptyEntries)
//                    .Select(x => uint.Parse(x))
//                    .ToList();
//            }

//            var iX = 0;
//            var iY = 0;
//            foreach (var gidHASHED in gids)
//            {
//                // Read out the flags
//                bool flipped_horizontally = (gidHASHED & FLIPPED_HORIZONTALLY_FLAG) != 0;
//                bool flipped_vertically = (gidHASHED & FLIPPED_VERTICALLY_FLAG) != 0;
//                bool flipped_diagonally = (gidHASHED & FLIPPED_DIAGONALLY_FLAG) != 0;

//                var gid = gidHASHED & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG);

//                var coords = tiledMap.ParseCoordinates(gid);

//                tiledLayer.Tiles.Add(new TiledPolygon(gid)
//                {
//                    FileName = tiledMap.TileFileNameByGid(gid),
//                    FlippedDiagonally = flipped_diagonally,
//                    FlippedHorizontally = flipped_horizontally,
//                    FlippedVertically = flipped_vertically,
//                    Layer = tiledLayer,
//                    TileOffsetX = coords.Xi,
//                    TileOffsetY = coords.Yi,
//                    Position = new Dot(iX, iY)
//                });

//                iX++;
//                if (iX > tiledLayer.width - 1)
//                {
//                    iX = 0;
//                    iY++;
//                }

//            }




//            string name = tiledLayer.GetTagAttrString("name");
//            bool isVisible = tiledLayer.ReadBoolean();
//            float opacity = tiledLayer.ReadSingle();
//            float x = tiledLayer.ReadSingle();
//            float y = tiledLayer.ReadSingle();
//            Vector2 vector = new Vector2(x, y);
//            float x2 = tiledLayer.ReadSingle();
//            float y2 = tiledLayer.ReadSingle();
//            Vector2 vector2 = new Vector2(x2, y2);
//            TiledMapProperties tiledMapProperties = new TiledMapProperties();
//            tiledLayer.ReadTiledMapProperties(tiledMapProperties);
//            TiledMapLayer tiledMapLayer = tiledMapLayerType switch
//            {
//                TiledMapLayerType.ImageLayer => ReadImageLayer(tiledLayer, name, type, vector, vector2, opacity, isVisible),
//                TiledMapLayerType.TileLayer => ReadTileLayer(tiledLayer, name, type, vector, vector2, opacity, isVisible, map),
//                TiledMapLayerType.ObjectLayer => ReadObjectLayer(tiledLayer, name, type, vector, vector2, opacity, isVisible, map),
//                TiledMapLayerType.GroupLayer => new TiledMapGroupLayer(name, type, ReadGroup(tiledLayer, map), vector, vector2, opacity, isVisible),
//                _ => throw new ArgumentOutOfRangeException(),
//            };
//            foreach (KeyValuePair<string, TiledMapPropertyValue> item in tiledMapProperties)
//            {
//                tiledMapLayer.Properties.Add(item.Key, item.Value);
//            }

//            return tiledMapLayer;
//        }

//        private static TiledMapLayer ReadObjectLayer(ContentReader reader, string name, string type, Vector2 offset, Vector2 parallaxFactor, float opacity, bool isVisible, TiledMap map)
//        {
//            Color value = reader.ReadColor();
//            TiledMapObjectDrawOrder drawOrder = (TiledMapObjectDrawOrder)reader.ReadByte();
//            int num = reader.ReadInt32();
//            TiledMapObject[] array = new TiledMapObject[num];
//            for (int i = 0; i < num; i++)
//            {
//                array[i] = ReadTiledMapObject(reader, map);
//            }

//            return new TiledMapObjectLayer(name, type, array, value, drawOrder, offset, parallaxFactor, opacity, isVisible);
//        }

//        private static TiledMapObject ReadTiledMapObject(ContentReader reader, TiledMap map)
//        {
//            TiledMapObjectType tiledMapObjectType = (TiledMapObjectType)reader.ReadByte();
//            int identifier = reader.ReadInt32();
//            string name = reader.ReadString();
//            string type = reader.ReadString();
//            Vector2 position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
//            float width = reader.ReadSingle();
//            float height = reader.ReadSingle();
//            SizeF size = new SizeF(width, height);
//            float rotation = reader.ReadSingle();
//            bool isVisible = reader.ReadBoolean();
//            TiledMapProperties tiledMapProperties = new TiledMapProperties();
//            reader.ReadTiledMapProperties(tiledMapProperties);
//            TiledMapObject tiledMapObject;
//            switch (tiledMapObjectType)
//            {
//                case TiledMapObjectType.Rectangle:
//                    tiledMapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, 1f, isVisible, type);
//                    break;
//                case TiledMapObjectType.Tile:
//                    {
//                        uint globalTileIdentifierWithFlags = reader.ReadUInt32();
//                        TiledMapTile tiledMapTile = new TiledMapTile(globalTileIdentifierWithFlags, (ushort)position.X, (ushort)position.Y);
//                        TiledMapTileset tilesetByTileGlobalIdentifier = map.GetTilesetByTileGlobalIdentifier(tiledMapTile.GlobalIdentifier);
//                        int localTileIdentifier = tiledMapTile.GlobalIdentifier - map.GetTilesetFirstGlobalIdentifier(tilesetByTileGlobalIdentifier);
//                        TiledMapTilesetTile tile = tilesetByTileGlobalIdentifier.Tiles.FirstOrDefault((TiledMapTilesetTile x) => x.LocalTileIdentifier == localTileIdentifier);
//                        tiledMapObject = new TiledMapTileObject(identifier, name, tilesetByTileGlobalIdentifier, tile, size, position, rotation, 1f, isVisible, type);
//                        break;
//                    }
//                case TiledMapObjectType.Ellipse:
//                    tiledMapObject = new TiledMapEllipseObject(identifier, name, size, position, rotation, 1f, isVisible);
//                    break;
//                case TiledMapObjectType.Polygon:
//                    tiledMapObject = new TiledMapPolygonObject(identifier, name, ReadPoints(reader), size, position, rotation, 1f, isVisible, type);
//                    break;
//                case TiledMapObjectType.Polyline:
//                    tiledMapObject = new TiledMapPolylineObject(identifier, name, ReadPoints(reader), size, position, rotation, 1f, isVisible, type);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }

//            foreach (KeyValuePair<string, TiledMapPropertyValue> item in tiledMapProperties)
//            {
//                tiledMapObject.Properties.Add(item.Key, item.Value);
//            }

//            return tiledMapObject;
//        }

//        private static Vector2[] ReadPoints(ContentReader reader)
//        {
//            int num = reader.ReadInt32();
//            Vector2[] array = new Vector2[num];
//            for (int i = 0; i < num; i++)
//            {
//                float x = reader.ReadSingle();
//                float y = reader.ReadSingle();
//                array[i] = new Vector2(x, y);
//            }

//            return array;
//        }

//        private static TiledMapImageLayer ReadImageLayer(ContentReader reader, string name, string type, Vector2 offset, Vector2 parallaxFactor, float opacity, bool isVisible)
//        {
//            Texture2D image = reader.ReadExternalReference<Texture2D>();
//            float x = reader.ReadSingle();
//            float y = reader.ReadSingle();
//            Vector2 value = new Vector2(x, y);
//            return new TiledMapImageLayer(name, type, image, value, offset, parallaxFactor, opacity, isVisible);
//        }

//        private static TiledMapTileLayer ReadTileLayer(ContentReader reader, string name, string type, Vector2 offset, Vector2 parallaxFactor, float opacity, bool isVisible, TiledMap map)
//        {
//            int num = reader.ReadInt32();
//            int height = reader.ReadInt32();
//            int tileWidth = map.TileWidth;
//            int tileHeight = map.TileHeight;
//            int num2 = reader.ReadInt32();
//            TiledMapTileLayer tiledMapTileLayer = new TiledMapTileLayer(name, type, num, height, tileWidth, tileHeight, offset, parallaxFactor, opacity, isVisible);
//            for (int i = 0; i < num2; i++)
//            {
//                uint globalTileIdentifierWithFlags = reader.ReadUInt32();
//                ushort num3 = reader.ReadUInt16();
//                ushort num4 = reader.ReadUInt16();
//                tiledMapTileLayer.Tiles[num3 + num4 * num] = new TiledMapTile(globalTileIdentifierWithFlags, num3, num4);
//            }

//            return tiledMapTileLayer;
//        }

//        public static void ReadTiledMapProperties(XElement tag, TiledMapProperties properties)
//        {
//            foreach (var prop in tag.Elements("property"))
//            {
//                var name = prop.GetTagAttrString("name");
//                var type = prop.GetTagAttrString("type");
//                var value = prop.GetTagAttrString("value");

//                var tiledProp = new TiledMapPropertyValue(value);

//#warning debug
//                ReadTiledMapProperties(prop, tiledProp.Properties);

//                properties[name] = tiledProp;
//            }
//        }
//    }

//}