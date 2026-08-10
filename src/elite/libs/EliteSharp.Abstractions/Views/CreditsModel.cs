// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The credits screen: who wrote the game, and which build this is.
/// <paramref name="Version"/> is read from the assembly, so it is content the
/// controller supplies rather than something the view can derive.
/// <para>
/// The screen has one control the commander can use - Back - and it is the
/// only row, so which row is selected is not worth a model field: the view
/// draws it selected because it always is.
/// </para>
/// </summary>
public sealed record CreditsModel(string Version, IReadOnlyList<string> Credits);
