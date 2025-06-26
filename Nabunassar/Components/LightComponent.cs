using Penumbra;

namespace Nabunassar.Components
{
    internal class LightComponent
    {
        private List<Light> _lights;

        private Dictionary<Light,Vector2> _lightPositions;

        /// <summary>
        /// <see cref="Light.Position"/> is relative of existed <see cref="MapObject"/>
        /// </summary>
        /// <param name="lights"></param>
        public LightComponent(params Light[] lights)
        {
            _lights =new List<Light>(lights);
            _lightPositions = _lights.ToDictionary(light => light, light => light.Position);
        }

        public void Move(Vector2 moving)
        {
            foreach (var light in _lights)
            {
                light.Position = _lightPositions[light] + moving;
            }
        }

        public bool IsAdded { get; set; }

        public void Disable()
        {
            _lights.ForEach(l=>l.Enabled = false);
        }

        public void Enable()
        {
            _lights.ForEach(_l=>_l.Enabled = true);
        }

        public IEnumerable<Light> Lights => _lights;
    }
}
