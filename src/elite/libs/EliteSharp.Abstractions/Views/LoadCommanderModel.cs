// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The load-commander screen: the name typed so far, and an error message
/// when the last attempt failed. <paramref name="ErrorMessage"/> is empty
/// when there is nothing to report, which the view takes as "don't draw it"
/// rather than carrying a separate visibility flag.
/// </summary>
public sealed record LoadCommanderModel(string Name, string ErrorMessage);
