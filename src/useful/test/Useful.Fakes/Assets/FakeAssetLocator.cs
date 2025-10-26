// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets;

namespace Useful.Fakes.Assets;

// Minimal IAssetLocator implementation for initialize benchmark.
public sealed class FakeAssetLocator : IAssetLocator
{
    public IDictionary<int, string> FontBitmapPaths { get; } = new Dictionary<int, string>();

    public IDictionary<int, string> FontTrueTypePaths { get; } = new Dictionary<int, string>();

    public IDictionary<int, string> ImagePaths { get; } = new Dictionary<int, string>();

    public bool IsInitialized { get; private set; }

    public IDictionary<int, string> MusicPaths { get; } = new Dictionary<int, string>();

    public IDictionary<int, string> SfxPaths { get; } = new Dictionary<int, string>();

    public void Initialize() => IsInitialized = true;
}
