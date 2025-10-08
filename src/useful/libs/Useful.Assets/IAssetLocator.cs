// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Useful.Assets;

public interface IAssetLocator
{
    public IDictionary<int, string> ImageAssets { get; }

    public IDictionary<int, string> SfxAssets { get; }

    public IDictionary<int, string> MusicAssets { get; }

    public IDictionary<int, string> FontAssets { get; }

    public IList<uint> Colors { get; }
}
