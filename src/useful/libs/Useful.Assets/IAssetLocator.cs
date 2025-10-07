// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Useful.Assets;

public interface IAssetLocator
{
    public IDictionary<string, string> ImageAssets { get; }

    public IDictionary<string, string> SfxAssets { get; }

    public IDictionary<string, string> MusicAssets { get; }

    public IDictionary<string, string> FontAssets { get; }

    public IDictionary<string, uint> Colors { get; }
}
