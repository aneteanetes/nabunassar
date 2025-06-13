using Nabunassar.Struct;

namespace Nabunassar.Resources
{
    internal class DataBase
    {
        NabunassarGame _game;

        public DataBase(NabunassarGame game)
        {
            _game = game;
        }

        public float GetGroundTypeSpeed(GroundType type)
        {
            var data = Get<Dictionary<GroundType, float>>("Data/Map/GroundTypeSpeed.json");
            return data[type];
        }

        public T Get<T>(string assetName)
        {
            return _game.Content.Load<T>(assetName);
        }
    }
}
