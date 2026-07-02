// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets;

namespace Useful.Fakes.Assets;

// Minimal IAssetLocator implementation for initialize benchmark.
public sealed class FakeAssetLocator : IAssetLocator
{
    // Points at the real palette shipped alongside the consuming project's output, since EliteDraw
    // reads it unconditionally in its constructor and has no fake substitute for palette colors.
    public string PalettePath { get; } = Path.Combine(AppContext.BaseDirectory, "Assets", "Palette", "palette.json");

    public IDictionary<string, string> FontBitmapPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> FontTrueTypePaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> ImagePaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> MusicPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> SfxPaths { get; } = new Dictionary<string, string>();

    public IDictionary<string, string> ModelPaths { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> SoundFontPaths { get; } = new Dictionary<string, string>();
}
