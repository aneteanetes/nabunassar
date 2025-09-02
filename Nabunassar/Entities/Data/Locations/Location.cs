using Nabunassar.Entities.Data.Enums;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Entities.Data.Locations
{
    internal class Location
    {
        private NabunassarGame _game;
        public Location(NabunassarGame game)
        {
            _game = game;
        }

        public string RegionName => _game.Strings["RegionNames"][Region.ToString()];

        public Region Region { get; set; }

        public TiledBase LoadedMap { get; set; }

        public IEnumerable<Gods> GetAvailableGods() => Region switch
        {
            Region.Congregation => [Gods.Nasho, Gods.Nergal, Gods.Nisa, Gods.Sabu, Gods.Teshrin, Gods.Ailul],
            Region.PrimordialGrove => [Gods.Sabu, Gods.Nasho, Gods.Teshrin, Gods.Rohati],
            Region.VoidBay => [Gods.Rohati, Gods.Teshrin, Gods.Sabu, Gods.Haya, Gods.Aval],
            Region.BloodySwamps => [Gods.Nisa, Gods.Nergal, Gods.Nasho, Gods.Sabu],
            Region.UshalEmpire => [Gods.Haya, Gods.Ailul, Gods.Teshrin, Gods.Rohati, Gods.Aval],
            Region.KingdomOfTheDamned => [Gods.Ailul, Gods.Haya, Gods.Teshrin],
            Region.MountainMeadows => [Gods.Tamus],
            Region.DilmunIsland => [Gods.Tamus, Gods.Shamadj],
            Region.ExarchateOfTheSun => [Gods.Aval, Gods.Ziran, Gods.Haya, Gods.Rohati],
            Region.ShoreOfTheAncients => [Gods.Ziran, Gods.Aval, Gods.Shamadj],
            Region.DarkForest => [Gods.Teshrin, Gods.Nasho, Gods.Sabu, Gods.Rohati, Gods.Haya],
            Region.SandsOfTheDead => [Gods.Nergal, Gods.Tamus, Gods.Nasho, Gods.Nisa],
            Region.Underdead => typeof(Gods).GetAllValues<Gods>().ToArray(),
            _ => Enumerable.Empty<Gods>(),
        };
    }
}