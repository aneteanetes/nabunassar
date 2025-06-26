using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Nabunassar.Components;
using Penumbra;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        Light light;

        public void LoadPenumbra()
        {
            var penumbraShaders = new Dictionary<string, Effect>();

            penumbraShaders["PenumbraHull"] = Content.Load<Effect>("Assets/Shaders/Penumbra/PenumbraHull.fx");
            penumbraShaders["PenumbraLight"] = Content.Load<Effect>("Assets/Shaders/Penumbra/PenumbraLight.fx");
            penumbraShaders["PenumbraShadow"] = Content.Load<Effect>("Assets/Shaders/Penumbra/PenumbraShadow.fx");
            penumbraShaders["PenumbraTexture"] = Content.Load<Effect>("Assets/Shaders/Penumbra/PenumbraTexture.fx");

            Penumbra = new PenumbraComponent(this, penumbraShaders);
            Penumbra.Initialize();
            Penumbra.Visible = false;
            Penumbra.AmbientColor = Color.Black;

            light = new PointLight
            {
                Scale = new Vector2(1000f), // Range of the light source (how far the light will travel)
                ShadowType = ShadowType.Solid // Will not lit hulls themselves,
            };


            var rec = new RectangleF(100, 100, 10, 10);


            Hull hull = new Hull(Vector2.Zero, new Vector2(rec.Width,0), new Vector2(rec.Width, rec.Height*-1), new Vector2(0, rec.Height * -1))
            {
                Position = rec.Position
            };
            //Penumbra.Hulls.Add(hull);
            //Penumbra.Lights.Add(light);

            Components.Add(Penumbra);
        }
    }    
}
