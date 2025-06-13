using Geranium.Reflection;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Xml.Linq;

namespace Nabunassar.Tiled.Map
{
    public class TiledMap
    {
        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
        const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
        const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

        private TiledMap() { }

        public static TiledMap Load(Stream stream)
        {
            var xdoc = XDocument.Load(stream);
            var map = xdoc.Root;

            var tiledMap = new TiledMap
            {
                width = map.GetTagAttrInteger(nameof(width)),
                height = map.GetTagAttrInteger(nameof(height)),
                tilewidth = map.GetTagAttrInteger(nameof(tilewidth)),
                tileheight = map.GetTagAttrInteger(nameof(tileheight)),
                infinite = map.GetTagAttrInteger(nameof(infinite)) == 1 ? true : false
            };

            foreach (var xmlTileSet in map.Elements("tileset"))
            {
                var tileSet = new TiledTileset()
                {
                    firstgid = xmlTileSet.GetTagAttrInteger(nameof(TiledTileset.firstgid)),
                    tilecount = xmlTileSet.GetTagAttrInteger(nameof(TiledTileset.tilecount)),
                    tilewidth = xmlTileSet.GetTagAttrInteger(nameof(TiledTileset.tilewidth)),
                    tileheight = xmlTileSet.GetTagAttrInteger(nameof(TiledTileset.tileheight)),
                    name = xmlTileSet.GetTagAttrString(nameof(TiledTileset.name)),
                };

                tileSet.Tiles = xmlTileSet
                    .Elements("tile")
                    .Select(x =>
                    {
                        var tile = new TiledTile()
                        {
                            Id = x.GetTagAttrInteger("id")
                        };

                        foreach (var prop in x.Element("properties").Elements("property"))
                        {
                            tile.Properties[prop.GetTagAttrString("name")] = prop.GetTagAttrString("value");
                        }

                        return tile;
                    })
                    .ToList();


                tileSet.Autotiled = true;
                tileSet.image = "Assets/Tilesets/" + Path.GetFileName(xmlTileSet.Element("image").GetTagAttrString("source"));


                tiledMap.Tilesets.Add(tileSet);
            }

            ProcessLayers(map, tiledMap);
            ProcessObjects(map, tiledMap);

            return tiledMap;
        }

        private static string ResourceFile(string file)
        {
            return file.Replace("../", "");
        }

        private static void ProcessObjects(XElement map, TiledMap tiledMap)
        {
            var objProps = typeof(TiledObject)
                .GetProperties()
                .Where(p => char.IsLower(p.Name[0]))
                .ToArray();

            var tiledObjProps = typeof(TiledObjectProperty)
                .GetProperties()
                .Where(p => char.IsLower(p.Name[0]))
                .ToArray();

            foreach (var objectlayer in map.Elements("objectgroup"))
            {
                foreach (var objtag in objectlayer.Elements())
                {
                    var tobj = new TiledObject()
                    {
                        objectgroup = objectlayer.Attribute("name").Value
                    };

                    foreach (var prop in objProps)
                    {
                        var attr = objtag.Attribute(prop.Name);
                        if (attr != null)
                        {
                            tobj.SetPropValue(prop.Name, attr.Value.Replace(".", ","));
                        }
                    }

                    tobj.Position = new Vector2((int)tobj.x, (int)tobj.y - 16);

                    if (tobj.gid != 0)
                    {
                        tobj.Tileset = tiledMap.GetTileset(tobj.gid);
                    }

                    var props = objtag.Element("properties");
                    if (props != default)
                    {
                        foreach (var xmlprop in props.Elements())
                        {
                            var p = new TiledObjectProperty();
                            foreach (var tiledObjProp in tiledObjProps)
                            {
                                var attr = xmlprop.Attribute(tiledObjProp.Name);
                                if (attr != null)
                                {
                                    p.SetPropValue(tiledObjProp.Name, attr.Value);
                                }
                            }
                            tobj.Properties.Add(p);
                        }
                    }

                    var xmlPolygon = objtag.Element("polygon");
                    if (xmlPolygon != null)
                    {
                        var xmlPoints = xmlPolygon.Attribute("points");
                        if (xmlPoints != null)
                        {
                            var pointsString = xmlPoints.Value;
                            tobj.Polygon = pointsString.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                .Select(pointstr =>
                                {
                                    var p = pointstr.Split(",", StringSplitOptions.RemoveEmptyEntries);
                                    return new Point(int.Parse(p[0]), int.Parse(p[1]));
                                }).ToArray();
                        }
                    }

                    if (tobj.objectgroup == "NPC")
                        tiledMap.NPCs.Add(tobj);
                    else
                        tiledMap.Objects.Add(tobj);
                }
            }
        }

        private static void ProcessLayers(XElement map, TiledMap tiledMap)
        {
            foreach (var xmlLayer in map.Elements("layer"))
            {
                var layer = new TiledLayer()
                {
                    name = xmlLayer.GetTagAttrString(nameof(TiledLayer.name)),
                    height = xmlLayer.GetTagAttrInteger(nameof(TiledLayer.height)),
                    width = xmlLayer.GetTagAttrInteger(nameof(TiledLayer.width))
                };

                var dataTag = xmlLayer.Element("data");

                var gids = new List<uint>();

                var chunks = dataTag.Elements("chunk");
                if (chunks.Count() > 0)
                {
                    chunks.ForEach(chunk =>
                    {
                        var gidsChunk = chunk.Value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => uint.Parse(x));
                        gids.AddRange(gidsChunk);
                    });
                }

                if (gids.Count == 0)
                {
                    gids = dataTag.Value
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => uint.Parse(x))
                        .ToList();
                }

                var iX = 0;
                var iY = 0;
                foreach (var gidHASHED in gids)
                {
                    // for saving position
                    //if (gidHASHED == 0)
                    //    continue;

                    // Read out the flags
                    bool flipped_horizontally = (gidHASHED & FLIPPED_HORIZONTALLY_FLAG) != 0;
                    bool flipped_vertically = (gidHASHED & FLIPPED_VERTICALLY_FLAG) != 0;
                    bool flipped_diagonally = (gidHASHED & FLIPPED_DIAGONALLY_FLAG) != 0;

                    var gid = gidHASHED & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG);

                    var coords = tiledMap.ParseCoordinates((int)gid);

                    var polygon = new TiledPolygon((int)gid)
                    {
                        FileName = tiledMap.TileFileNameByGid(gid),
                        FlippedDiagonally = flipped_diagonally,
                        FlippedHorizontally = flipped_horizontally,
                        FlippedVertically = flipped_vertically,
                        Layer = layer,
                        TileOffsetX = coords.X,
                        TileOffsetY = coords.Y,
                        Position = new Vector2(iX*16, iY*16),
                        Tileset = gid == 0 ? null : tiledMap.GetTileset((int)gid)
                    };

                    layer.Tiles.Add(polygon);

                    if (gid != 0)
                        polygon.Properties = polygon.Tileset.GetTileProperties(polygon.Gid-1);

                    iX++;
                    if (iX > layer.width - 1)
                    {
                        iX = 0;
                        iY++;
                    }

                }

                layer.TilesArray = layer.Tiles.GroupBy(x => x.Position.Y)
                    .Select(x => new List<TiledPolygon>(x.OrderBy(x => x.Position.X)))
                    .ToList();

                tiledMap.Layers.Add(layer);
            }
        }

        public bool infinite { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public int tilewidth { get; set; }

        public int tileheight { get; set; }

        public List<TiledObject> Objects { get; set; } = new List<TiledObject>();

        public List<TiledObject> NPCs { get; set; } = new List<TiledObject>();

        public List<TiledTileset> Tilesets { get; set; } = new List<TiledTileset>();

        private Point ParseCoordinates(int gid)
        {
            var tileset = Tilesets.FirstOrDefault(x => x.IsContainsGid(gid));
            if (tileset != null)
                return tileset.Coords(gid);

            return Point.Zero;
        }

        private TiledTileset GetTileset(int gid)
        {
            var tileset = Tilesets.FirstOrDefault(x => x.IsContainsGid(gid));
            if (tileset != null)
                return tileset;

            throw new FileNotFoundException("Gid from unknown tileset!");
        }

        private string TileFileNameByGid(uint gid)
        {
            if (gid == 0)
                return null;

            var tileset = Tilesets.FirstOrDefault(x => x.TileGids.Contains(gid));

            if (tileset.Autotiled)
                return tileset.name;


            return null;
#warning tileset tile filename
            //return tileset.Tiles
            //    .FirstOrDefault(x => x.Id == Math.Abs(gid - tileset.firstgid))
            //    ?.File;
        }

        public List<TiledLayer> Layers { get; set; } = new List<TiledLayer>();
    }
}