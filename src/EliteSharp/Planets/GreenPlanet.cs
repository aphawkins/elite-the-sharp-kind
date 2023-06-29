// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp.Planets
{
    internal sealed class GreenPlanet : PlanetRenderer
    {
        internal GreenPlanet(IGraphics graphics, IDraw draw)
            : base(graphics, draw)
        {
        }

        public override void Draw(Vector2 centre, float radius, Vector3[] vec) => _graphics.DrawCircleFilled(centre, radius, Colour.Green);
    }
}
