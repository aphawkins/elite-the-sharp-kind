// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Fakes.Assets;

// Minimal IAssetLocator implementation for initialize benchmark.
public sealed class FakeAssetLocator : IAssetLocator
{
    public SystemTier Tier => SystemTier.SixteenBit;

    // Points at the real palette shipped alongside the consuming project's output, since EliteDraw
    // reads it unconditionally in its constructor and has no fake substitute for palette colors.
    // Built by hand rather than through AssetLocator: consumers that never touch the palette (the
    // audio tests) have no asset manifest to read, so this must stay a plain string.
    public string PalettePath { get; } =
        Path.Combine(AppContext.BaseDirectory, "Assets", "Palette", nameof(SystemTier.SixteenBit), "palette.json");

    public IDictionary<string, string> FontBitmapPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes { get; } = new Dictionary<string, TrueTypeFontAsset>();

    public IDictionary<string, string> ImagePaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> MusicPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> SfxPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> ModelPaths { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> SoundFontPaths { get; } = new Dictionary<string, string>();
}
