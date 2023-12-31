// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Media;
using System.Runtime.Versioning;
using EliteSharp.Audio;

namespace EliteSharp.WinForms
{
    [SupportedOSPlatform("windows")]
    internal sealed class WinSound(GDIAssetLoader assetLoader) : ISound
    {
        private readonly Dictionary<SoundEffect, SoundPlayer> _sfx = assetLoader.LoadSfx();
        private readonly Dictionary<MusicType, SoundPlayer> _music = assetLoader.LoadMusic();
        private bool _disposedValue;

        public void Play(SoundEffect sfxType) => _sfx[sfxType].Play();

        public void Play(MusicType musicType, bool repeat)
        {
            StopMusic();

            if (repeat)
            {
                _music[musicType].PlayLooping();
            }
            else
            {
                _music[musicType].Play();
            }
        }

        public void StopMusic()
        {
            foreach (KeyValuePair<MusicType, SoundPlayer> music in _music)
            {
                music.Value.Stop();
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    foreach (KeyValuePair<MusicType, SoundPlayer> v in _music)
                    {
                        v.Value.Dispose();
                    }

                    foreach (KeyValuePair<SoundEffect, SoundPlayer> v in _sfx)
                    {
                        v.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }
    }
}
