// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.UI;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// What a settings list looks like in one rendition: every colour and every
/// position it needs, and nothing else. The game builds the controls and binds
/// them to its settings; the rendition says only how they are to look, which
/// is the same division ShipColours and the planet renderers already use.
/// <para>
/// This is why there is no settings view per rendition. A screen whose whole
/// tier-specific content is a set of numbers does not need an assembly's
/// worth of drawing code in each rendition to express them.
/// </para>
/// </summary>
/// <param name="RowStyle">
/// The row's font and colours. Its selected background is the cursor block,
/// which fills the whole row.
/// </param>
/// <param name="ValueStyle">
/// The value's and arrows' colours. Backgrounds should be transparent: the
/// row's own has already been filled.
/// </param>
/// <param name="RowsLeft">
/// The left edge of the setting rows. The Back row and the footer are centred
/// on the viewport instead, so they need no equivalent.
/// </param>
/// <param name="FirstRowY">The top of the first setting row.</param>
/// <param name="RowHeight">The height of a row, and the pitch between them.</param>
/// <param name="RowWidth">The width of a setting row, which its block fills.</param>
/// <param name="ValueOffsetX">The value's distance from the row's left edge.</param>
/// <param name="ArrowGap">The space between a cycling arrow and the value.</param>
/// <param name="BackRowWidth">
/// The width of the Back row's block. It carries no value and no arrows, so
/// it is usually narrower than a setting's.
/// </param>
/// <param name="BackRowY">The top of the Back row.</param>
/// <param name="FooterY">The top of the footer note, if the screen has one.</param>
/// <param name="SnapToCell">
/// The character cell centred text is rounded to, or zero for a proportional
/// font that has no grid to sit on.
/// </param>
public sealed record SettingsListStyle(
    ControlStyle RowStyle,
    ControlStyle ValueStyle,
    float RowsLeft,
    float FirstRowY,
    float RowHeight,
    float RowWidth,
    float ValueOffsetX,
    float ArrowGap,
    float BackRowWidth,
    float BackRowY,
    float FooterY,
    float SnapToCell);
