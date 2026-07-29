// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// Everything the galactic chart draws, with no layout applied: all
/// positions are in galaxy space - the raw (D, B) of a
/// <see cref="Types.GalaxySeed"/>, 0-255 on each axis - so each tier's view
/// applies its own scaling, and the fuel radius is left in light years for
/// the same reason.
/// </summary>
internal sealed record GalacticChartModel(
    string Title,
    IReadOnlyList<GalacticChartStar> Stars,
    Vector2 DockedPlanet,
    float FuelLightYears,
    Vector2 Cross,
    string Caption,
    string Detail);
