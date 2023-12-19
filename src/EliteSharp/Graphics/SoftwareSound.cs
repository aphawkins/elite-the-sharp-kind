// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;

namespace EliteSharp.Graphics
{
    public sealed class SoftwareSound(SoftwareAssetLoader assetLoader) : ISound
    {
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable RCS1213 // Remove unused member declaration
        private readonly Dictionary<SoundEffect, EWave> _sfx = assetLoader.LoadSfx();
        private readonly Dictionary<MusicType, EWave> _music = assetLoader.LoadMusic();
#pragma warning restore RCS1213 // Remove unused member declaration
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CA1823 // Avoid unused private fields

        public void Dispose()
        {
        }

        public void Play(MusicType musicType, bool repeat)
        {
        }

        public void Play(SoundEffect sfxType)
        {
        }

        public void StopMusic()
        {
        }
    }
}
