// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.UI;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// A fixed caption a rendition puts somewhere: its words and where they go. The
/// column headings on a list screen are these, so a rendition that wants
/// "QUANTITY" over two lines says so by supplying two of them rather than the
/// game knowing how many lines a heading takes.
/// </summary>
/// <param name="Text">The words, which never change.</param>
/// <param name="X">Where they sit across the screen.</param>
/// <param name="Y">Where they sit down it.</param>
/// <param name="Alignment">Which end of the text sits on <paramref name="X"/>.</param>
public readonly record struct PlacedText(string Text, float X, float Y, TextAlignment Alignment);
