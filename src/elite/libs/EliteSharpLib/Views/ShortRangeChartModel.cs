// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// Everything the short range chart draws.
/// <para>
/// Unlike <see cref="GalacticChartModel"/>, the positions here are already in
/// screen space. The chart's row packing decides which planets get a name at
/// all, and it packs against text rows, so the layout cannot be deferred to
/// the view without splitting the packing in two - and the blob sizes depend
/// on <c>GameState.CarryFlag</c>, which only moves when a name wins a free
/// row. The controller therefore lays out against the tier's metrics and
/// hands the view finished positions, keeping that quirk in one place.
/// </para>
/// </summary>
internal sealed record ShortRangeChartModel(
    string Title,
    IReadOnlyList<ShortRangeChartPlanet> Planets,
    IReadOnlyList<ShortRangeChartLabel> Labels,
    float FuelLightYears,
    Vector2 Cross,
    string Caption,
    string Detail);
