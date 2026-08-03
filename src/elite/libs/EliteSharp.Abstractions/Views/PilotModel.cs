// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Ships;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// One cockpit window: its name, the hyperspace countdown text (empty when
/// there is none to show), and this direction's laser mount. The starfield
/// and the ship ahead are drawn by the universe, not through this model.
/// </summary>
/// <param name="ViewName">Which way the commander is looking.</param>
/// <param name="HyperspaceStatus">
/// The countdown line, or empty when there is no jump running.
/// </param>
/// <param name="LaserType">The mount this direction carries.</param>
/// <param name="IsFiring">Whether the beams are drawn this frame.</param>
/// <param name="LaserAim">
/// Where the beams converge, relative to the viewport centre and in the
/// original's coordinates. It is a fresh roll each frame, which is what makes
/// the beams shimmer, and the game rolls it because the game owns the one
/// source of entropy - a view that rolled its own would not be reproducible.
/// </param>
/// <param name="LaserWireframe">
/// Whether the beams are outlined rather than filled, which follows the
/// graphics style the commander configured.
/// </param>
public sealed record PilotModel(
    string ViewName,
    string HyperspaceStatus,
    LaserType LaserType,
    bool IsFiring,
    Vector2 LaserAim,
    bool LaserWireframe);
