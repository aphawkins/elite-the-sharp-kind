// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Useful.Assets;

public interface IAssetLocator
{
    public IDictionary<int, string> FontBitmapPaths { get; }

    public IDictionary<int, string> FontTrueTypePaths { get; }

    public IDictionary<int, string> ImagePaths { get; }

    public bool IsInitialized { get; }

    public IDictionary<int, string> MusicPaths { get; }

    public IDictionary<int, string> SfxPaths { get; }

    public void Initialize();
}
