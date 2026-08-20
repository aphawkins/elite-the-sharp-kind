// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets;

/// <summary>
/// A Windows .fon font asset: where the file lives and which strike to take
/// from it. Which sizes exist belongs to the file; which one is wanted
/// belongs to the game's asset manifest, not to the rendering library.
/// </summary>
/// <param name="Path">Full path to the .fon file.</param>
/// <param name="PixelHeight">Pixel height of the strike to use; the nearest one in the file is taken.</param>
public readonly record struct FonFontAsset(string Path, int PixelHeight);
