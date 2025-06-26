using Penumbra;

namespace Nabunassar.Components
{
    internal class HullComponent
    {
        private List<Hull> _hulls;
        private Dictionary<Hull, Vector2> _hullsPositions;

        /// <summary>
        /// <see cref="Hull.Position"/> is relative of existed <see cref="MapObject"/>
        /// </summary>
        /// <param name="hulls"></param>
        public HullComponent(params Hull[] hulls)
        {
            _hulls=new List<Hull>(hulls);
            _hullsPositions = _hulls.ToDictionary(light => light, light => light.Position);
        }

        public void Move(Vector2 moving)
        {
            foreach (var hull in _hulls)
            {
                hull.Position = _hullsPositions[hull] + moving;
            }
        }

        public bool IsAdded { get; set; }

        public void Disable()
        {
            _hulls.ForEach(l => l.Enabled = false);
        }

        public void Enable()
        {
            _hulls.ForEach(_l => _l.Enabled = true);
        }

        public IEnumerable<Hull> Hulls => _hulls;
    }
}
