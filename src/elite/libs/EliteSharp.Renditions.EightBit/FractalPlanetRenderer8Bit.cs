// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Abstractions.Views.Planets;
using Useful;

namespace EliteSharp.Renditions.EightBit;

/// <summary>
/// The generated planet: land above the waterline, sea below, each in a lit
/// and a shaded shade. Four colours is all this palette can spare for it.
/// </summary>
internal sealed class FractalPlanetRenderer8Bit : FractalPlanetRendererBase
{
    // The height the surface stops being sea and starts being land.
    private const uint Waterline = 166;

    private readonly FastColor _darkSea;
    private readonly FastColor _darkLand;
    private readonly FastColor _sea;
    private readonly FastColor _land;

    internal FractalPlanetRenderer8Bit(IViewSurface surface, IRandomSource random)
        : base(surface, random)
    {
        _darkSea = surface.Palette["Blue"];
        _darkLand = surface.Palette["Green"];
        _sea = surface.Palette["LightBlue"];
        _land = surface.Palette["LightGreen"];
    }

    protected override FastColor SurfaceColour(uint height, bool isShaded)
        => height > Waterline
            ? (isShaded ? _darkLand : _land)
            : (isShaded ? _darkSea : _sea);
}
