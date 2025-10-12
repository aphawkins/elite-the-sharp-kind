// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Assets;

namespace Useful.Audio;

public interface ISound
{
    public bool IsInitialized { get; }

    public void Initialize(IAssetLocator assetLocator);

    public void Play(int musicType, bool repeat);

    public void Play(int sfxType);

    public void StopMusic();
}
