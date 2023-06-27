// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp.Planets
{
    internal sealed class SnesPlanet : PlanetRenderer
    {
        /// <summary>
        /// Colour map used to generate a SNES Elite style planet.
        /// </summary>
        private readonly Colour[] _snesPlanetColour = new Colour[]
        {
            Colour.Purple,
            Colour.Purple,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.LightBlue,
            Colour.LightBlue,
            Colour.LighterGrey,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.DarkOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.LightOrange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.Orange,
            Colour.LighterGrey,
            Colour.LightBlue,
            Colour.LightBlue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.Blue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.DarkBlue,
            Colour.Purple,
            Colour.Purple,
        };

        internal SnesPlanet(IGraphics graphics, IDraw draw)
            : base(graphics, draw) => GenerateLandscape();

        /// <summary>
        /// Generate a landscape map for a SNES Elite style planet.
        /// </summary>
        private void GenerateLandscape()
        {
            for (int y = 0; y <= LANDYMAX; y++)
            {
                int colour = (int)_snesPlanetColour[y * (_snesPlanetColour.Length - 1) / LANDYMAX];
                for (int x = 0; x <= LANDXMAX; x++)
                {
                    _landscape[x, y] = colour;
                }
            }
        }
    }
}
