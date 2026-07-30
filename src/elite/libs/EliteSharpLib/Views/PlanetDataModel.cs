// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The planet data screen's formatted fields. <paramref name="Distance"/> is
/// empty until the commander has actually plotted a course, which the view
/// takes as "don't draw the row" rather than carrying a separate flag.
/// </summary>
internal sealed record PlanetDataModel(
    string Header,
    string Distance,
    string Economy,
    string Government,
    string TechLevel,
    string Population,
    string Productivity,
    string Radius,
    string Description);
