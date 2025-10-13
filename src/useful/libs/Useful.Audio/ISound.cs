// 'Useful Libraries' - Andy Hawkins 2025.

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
