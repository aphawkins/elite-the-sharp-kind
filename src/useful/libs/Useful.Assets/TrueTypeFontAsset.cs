// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

/// <summary>
/// A TrueType font asset: where the file lives and the point size it is opened at.
/// The size belongs to the game's asset manifest, not to the rendering library.
/// </summary>
/// <param name="Path">Full path to the .ttf file.</param>
/// <param name="PointSize">Point size to open the font at.</param>
public readonly record struct TrueTypeFontAsset(string Path, int PointSize);
